using FluentResults;
using Nova.Domain.Models;
using Nova.Midi;
using Serilog;

namespace Nova.Application.UseCases;

/// <summary>
/// Use case for saving a single preset to a specific user preset number on the hardware via MIDI.
/// </summary>
public sealed class SavePresetUseCase : ISavePresetUseCase
{
    private readonly IMidiPort _midiPort;
    private readonly IRequestPresetUseCase _requestPresetUseCase;
    private readonly ILogger _logger;

    public SavePresetUseCase(IMidiPort midiPort, IRequestPresetUseCase requestPresetUseCase, ILogger logger)
    {
        _midiPort = midiPort;
        _requestPresetUseCase = requestPresetUseCase;
        _logger = logger;
    }

    /// <summary>
    /// Saves the preset to the specified user preset number on the connected Nova System pedal.
    /// </summary>
    /// <param name="preset">The preset to save</param>
    /// <param name="presetNumber">Target preset number (31-90)</param>
    /// <param name="verify">If true, requests the preset back after saving to verify it was saved correctly (default: false)</param>
    /// <returns>Result indicating success or failure</returns>
    public async Task<Result> ExecuteAsync(Preset preset, int presetNumber, bool verify = false)
    {
        try
        {
            // Validate preset number (user bank 31-90)
            if (presetNumber < 31 || presetNumber > 90)
            {
                _logger.Error("Invalid preset number: {PresetNumber}. Must be between 31 and 90", presetNumber);
                return Result.Fail($"Invalid preset number: {presetNumber}. Must be between 31 and 90");
            }

            // Check connection
            if (!_midiPort.IsConnected)
            {
                _logger.Error("Cannot save preset: MIDI port not connected");
                return Result.Fail("Not connected to Nova System. Please connect first.");
            }

            _logger.Information("Saving preset '{PresetName}' to preset #{PresetNumber}", preset.Name, presetNumber);

            // Clone preset with updated preset number
            // Since Preset uses private setters, we serialize/deserialize to create a copy with new Number
            var originalSysex = preset.ToSysEx();
            if (originalSysex.IsFailed)
            {
                _logger.Error("Failed to serialize original preset: {Errors}", string.Join(", ", originalSysex.Errors));
                return Result.Fail($"Failed to serialize preset: {string.Join(", ", originalSysex.Errors)}");
            }

            // Update preset number in SysEx (byte 8)
            var sysexData = originalSysex.Value;
            sysexData[8] = (byte)presetNumber;

            // Recalculate checksum (byte 518 = sum of bytes 34-517 & 0x7F)
            int checksum = 0;
            for (int i = 34; i <= 517; i++)
            {
                checksum += sysexData[i];
            }
            sysexData[518] = (byte)(checksum & 0x7F);

            _logger.Debug("Updated SysEx to {ByteCount} bytes with preset #{PresetNumber}", sysexData.Length, presetNumber);

            // Validate SysEx format
            if (sysexData.Length != 520)
            {
                _logger.Error("Invalid SysEx length: {Length} bytes (expected 520)", sysexData.Length);
                return Result.Fail($"Invalid SysEx format: {sysexData.Length} bytes (expected 520)");
            }

            if (sysexData[0] != 0xF0 || sysexData[519] != 0xF7)
            {
                _logger.Error("Invalid SysEx markers: start={Start:X2}, end={End:X2}", sysexData[0], sysexData[519]);
                return Result.Fail("Invalid SysEx format: missing F0/F7 markers");
            }

            // Send to hardware
            var sendResult = await _midiPort.SendSysExAsync(sysexData);
            if (sendResult.IsFailed)
            {
                _logger.Error("Failed to send preset to hardware: {Errors}", string.Join(", ", sendResult.Errors));
                return Result.Fail($"Failed to send preset to hardware: {sendResult.Errors.First().Message}");
            }

            _logger.Information("Successfully sent preset '{PresetName}' to preset #{PresetNumber}", preset.Name, presetNumber);

            // Step 7: Verify save if requested
            if (verify)
            {
                _logger.Information("Verifying preset save by requesting it back from hardware");
                
                // Wait a moment for hardware to process the save (typically < 100ms)
                await Task.Delay(200);
                
                var verifyResult = await _requestPresetUseCase.ExecuteAsync(presetNumber, timeout: 3000);
                if (verifyResult.IsFailed)
                {
                    _logger.Error("Verification failed: could not read preset back from hardware: {Errors}", 
                        string.Join(", ", verifyResult.Errors));
                    return Result.Fail($"Save succeeded but verification failed: {verifyResult.Errors.First().Message}");
                }

                // Compare the saved SysEx with the retrieved SysEx
                var retrievedSysEx = verifyResult.Value.ToSysEx().Value;
                
                // Compare all bytes (exact match required)
                bool matches = sysexData.SequenceEqual(retrievedSysEx);
                
                if (!matches)
                {
                    _logger.Error("Verification failed: saved preset does not match retrieved preset");
                    // Log first difference for debugging
                    for (int i = 0; i < Math.Min(sysexData.Length, retrievedSysEx.Length); i++)
                    {
                        if (sysexData[i] != retrievedSysEx[i])
                        {
                            _logger.Error("First difference at byte {Index}: sent={Sent:X2}, received={Received:X2}", 
                                i, sysexData[i], retrievedSysEx[i]);
                            break;
                        }
                    }
                    return Result.Fail("Save succeeded but verification failed: preset data mismatch. " +
                        "The preset may not have been saved correctly to hardware.");
                }

                _logger.Information("Verification successful: preset matches hardware");
            }

            _logger.Information("Successfully saved preset '{PresetName}' to preset #{PresetNumber}", preset.Name, presetNumber);
            return Result.Ok();
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Unexpected error saving preset #{PresetNumber}", presetNumber);
            return Result.Fail($"Unexpected error: {ex.Message}");
        }
    }
}
