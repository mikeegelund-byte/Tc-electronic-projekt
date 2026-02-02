using FluentResults;
using Nova.Domain.Models;
using Nova.Midi;
using Serilog;

namespace Nova.Application.UseCases;

/// <summary>
/// Use case for saving a single preset to a specific slot on the hardware via MIDI.
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
    /// Saves the preset to the specified slot on the connected Nova System pedal.
    /// </summary>
    /// <param name="preset">The preset to save</param>
    /// <param name="slotNumber">Target slot number (1-60)</param>
    /// <param name="verify">If true, requests the preset back after saving to verify it was saved correctly (default: false)</param>
    /// <returns>Result indicating success or failure</returns>
    public async Task<Result> ExecuteAsync(Preset preset, int slotNumber, bool verify = false)
    {
        try
        {
            // Validate slot number
            if (slotNumber < 1 || slotNumber > 60)
            {
                _logger.Error("Invalid slot number: {SlotNumber}. Must be between 1 and 60", slotNumber);
                return Result.Fail($"Invalid slot number: {slotNumber}. Must be between 1 and 60");
            }

            // Check connection
            if (!_midiPort.IsConnected)
            {
                _logger.Error("Cannot save preset: MIDI port not connected");
                return Result.Fail("Not connected to Nova System. Please connect first.");
            }

            _logger.Information("Saving preset '{PresetName}' to slot {SlotNumber}", preset.Name, slotNumber);

            // Clone preset with updated slot number
            // Since Preset uses private setters, we serialize/deserialize to create a copy with new Number
            var originalSysex = preset.ToSysEx();
            if (originalSysex.IsFailed)
            {
                _logger.Error("Failed to serialize original preset: {Errors}", string.Join(", ", originalSysex.Errors));
                return Result.Fail($"Failed to serialize preset: {string.Join(", ", originalSysex.Errors)}");
            }

            // Update slot number in SysEx (byte 8)
            var sysexData = originalSysex.Value;
            sysexData[8] = (byte)slotNumber;

            // Recalculate checksum (byte 518 = sum of bytes 34-517 & 0x7F)
            int checksum = 0;
            for (int i = 34; i <= 517; i++)
            {
                checksum += sysexData[i];
            }
            sysexData[518] = (byte)(checksum & 0x7F);

            _logger.Debug("Updated SysEx to {ByteCount} bytes with slot {SlotNumber}", sysexData.Length, slotNumber);

            // Validate SysEx format
            if (sysexData.Length != 521)
            {
                _logger.Error("Invalid SysEx length: {Length} bytes (expected 521)", sysexData.Length);
                return Result.Fail($"Invalid SysEx format: {sysexData.Length} bytes (expected 521)");
            }

            if (sysexData[0] != 0xF0 || sysexData[520] != 0xF7)
            {
                _logger.Error("Invalid SysEx markers: start={Start:X2}, end={End:X2}", sysexData[0], sysexData[520]);
                return Result.Fail("Invalid SysEx format: missing F0/F7 markers");
            }

            // Send to hardware
            var sendResult = await _midiPort.SendSysExAsync(sysexData);
            if (sendResult.IsFailed)
            {
                _logger.Error("Failed to send preset to hardware: {Errors}", string.Join(", ", sendResult.Errors));
                return Result.Fail($"Failed to send preset to hardware: {sendResult.Errors.First().Message}");
            }

            _logger.Information("Successfully sent preset '{PresetName}' to slot {SlotNumber}", preset.Name, slotNumber);

            // Step 7: Verify save if requested
            if (verify)
            {
                _logger.Information("Verifying preset save by requesting it back from hardware");
                
                // Wait a moment for hardware to process the save (typically < 100ms)
                await Task.Delay(200);
                
                var verifyResult = await _requestPresetUseCase.ExecuteAsync(slotNumber, timeout: 3000);
                if (verifyResult.IsFailed)
                {
                    _logger.Error("Verification failed: could not read preset back from hardware: {Errors}", 
                        string.Join(", ", verifyResult.Errors));
                    return Result.Fail($"Save succeeded but verification failed: {verifyResult.Errors.First().Message}");
                }

                // Compare the saved SysEx with the retrieved SysEx
                var retrievedSysEx = verifyResult.Value.ToSysEx().Value;
                
                // Compare all bytes except the checksum (bytes 518-519 might vary slightly due to rounding)
                // Actually, compare everything - if it doesn't match exactly, something went wrong
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

            _logger.Information("Successfully saved preset '{PresetName}' to slot {SlotNumber}", preset.Name, slotNumber);
            return Result.Ok();
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Unexpected error saving preset to slot {SlotNumber}", slotNumber);
            return Result.Fail($"Unexpected error: {ex.Message}");
        }
    }
}
