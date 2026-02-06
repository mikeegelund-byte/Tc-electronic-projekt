using FluentResults;
using Nova.Domain.Midi;
using Nova.Domain.Models;
using Nova.Midi;
using Serilog;
using System.Diagnostics;

namespace Nova.Application.UseCases;

/// <summary>
/// Use case for requesting a single preset from the hardware.
/// Sends a SysEx request and waits for the response with timeout.
/// </summary>
public sealed class RequestPresetUseCase : IRequestPresetUseCase
{
    private readonly IMidiPort _midiPort;
    private readonly ILogger _logger;

    public RequestPresetUseCase(IMidiPort midiPort, ILogger logger)
    {
        _midiPort = midiPort;
        _logger = logger;
    }

    /// <summary>
    /// Requests a specific preset from the hardware device.
    /// </summary>
    /// <param name="slotNumber">User slot number (1-60)</param>
    /// <param name="timeout">Timeout in milliseconds (default: 2000ms)</param>
    /// <returns>Result containing the preset on success, or error on failure</returns>
    public async Task<Result<Preset>> ExecuteAsync(int slotNumber, int timeout = 2000)
    {
        try
        {
            // Step 1: Validate slot number
            if (slotNumber < 1 || slotNumber > 60)
            {
                _logger.Error("Invalid slot number: {SlotNumber}. Must be 1-60", slotNumber);
                return Result.Fail<Preset>($"Slot number must be between 1 and 60, got {slotNumber}");
            }

            // Step 2: Check MIDI connection
            if (!_midiPort.IsConnected)
            {
                _logger.Error("MIDI port not connected");
                return Result.Fail<Preset>("MIDI device not connected");
            }

            _logger.Information("Requesting preset from slot {SlotNumber}", slotNumber);

            // Step 3: Convert slot number to preset number (User presets are 31-90, so slot 1-60 => preset 31-90)
            byte presetNumber = (byte)(slotNumber + 30);  // Slot 1 => Preset 31 (0x1F), Slot 60 => Preset 90 (0x5A)

            // Step 4: Build request SysEx
            byte[] requestSysEx = SysExBuilder.BuildPresetRequest(presetNumber);

            _logger.Debug("Sending preset request for slot {SlotNumber} (preset #{PresetNumber})", slotNumber, presetNumber);

            // Step 5: Start listening for SysEx responses BEFORE sending request
            var cancellationTokenSource = new CancellationTokenSource(timeout);
            var receiveTask = Task.Run(async () =>
            {
                await foreach (var sysex in _midiPort.ReceiveSysExAsync(cancellationTokenSource.Token))
                {
                    _logger.Debug("Received SysEx message: {Length} bytes", sysex.Length);
                    
                    // Check if this is a valid preset response (521 bytes, F0/F7 markers)
                    if (sysex.Length == 521 && sysex[0] == 0xF0 && sysex[520] == 0xF7)
                    {
                        _logger.Debug("Valid preset SysEx received");
                        return sysex;
                    }
                    
                    _logger.Debug("Ignoring non-preset SysEx message");
                }
                
                // If we exit the loop without finding a preset, timeout occurred
                throw new TimeoutException("No preset response received");
            }, cancellationTokenSource.Token);

            // Step 6: Send request after starting listener
            var sendResult = await _midiPort.SendSysExAsync(requestSysEx);
            if (sendResult.IsFailed)
            {
                cancellationTokenSource.Cancel();
                _logger.Error("Failed to send preset request: {Errors}", string.Join(", ", sendResult.Errors));
                return Result.Fail<Preset>($"Failed to send request: {string.Join(", ", sendResult.Errors)}");
            }

            _logger.Debug("Sent preset request, waiting for response (timeout: {Timeout}ms)", timeout);

            // Step 7: Wait for response with timeout
            try
            {
                byte[] responseSysEx = await receiveTask;
                _logger.Information("Received preset response from slot {SlotNumber}", slotNumber);

                // Step 8: Parse preset
                var presetResult = Preset.FromSysEx(responseSysEx);
                if (presetResult.IsFailed)
                {
                    _logger.Error("Failed to parse preset response: {Errors}", string.Join(", ", presetResult.Errors));
                    return Result.Fail<Preset>($"Failed to parse preset: {string.Join(", ", presetResult.Errors)}");
                }

                return Result.Ok(presetResult.Value);
            }
            finally
            {
                cancellationTokenSource.Dispose();
            }
        }
        catch (OperationCanceledException)
        {
            _logger.Warning("Preset request timed out after {Timeout}ms", timeout);
            return Result.Fail<Preset>($"Request timed out after {timeout}ms. Device may not be responding.");
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Unexpected error requesting preset from slot {SlotNumber}", slotNumber);
            return Result.Fail<Preset>($"Unexpected error: {ex.Message}");
        }
    }
}
