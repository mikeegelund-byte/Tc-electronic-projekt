using FluentResults;
using Nova.Domain.Models;
using Serilog;

namespace Nova.Application.UseCases;

/// <summary>
/// Use case for deleting a preset by clearing its slot.
/// </summary>
public sealed class DeletePresetUseCase : IDeletePresetUseCase
{
    private readonly ISavePresetUseCase _savePresetUseCase;
    private readonly ILogger _logger;

    public DeletePresetUseCase(
        ISavePresetUseCase savePresetUseCase,
        ILogger logger)
    {
        _savePresetUseCase = savePresetUseCase;
        _logger = logger;
    }

    /// <summary>
    /// Deletes a preset by sending an init preset with default parameters to clear the slot.
    /// </summary>
    /// <param name="slotNumber">Slot number (1-60)</param>
    /// <returns>Result indicating success or failure</returns>
    public async Task<Result> ExecuteAsync(int slotNumber)
    {
        try
        {
            // Validate slot number
            if (slotNumber < 1 || slotNumber > 60)
            {
                _logger.Error("Invalid slot number for delete: {SlotNumber}", slotNumber);
                return Result.Fail($"Slot number must be between 1 and 60 (got {slotNumber})");
            }

            _logger.Information("Deleting preset in slot {SlotNumber}", slotNumber);

            // Create init preset SysEx with default parameters
            var initPresetSysEx = CreateInitPresetSysEx(slotNumber);

            // Parse to Preset object
            var presetResult = Preset.FromSysEx(initPresetSysEx);
            if (presetResult.IsFailed)
            {
                _logger.Error("Failed to create init preset: {Errors}", 
                    string.Join(", ", presetResult.Errors));
                return Result.Fail($"Failed to create init preset: {presetResult.Errors.First().Message}");
            }

            // Send init preset to hardware to clear the slot
            var saveResult = await _savePresetUseCase.ExecuteAsync(presetResult.Value, slotNumber, verify: false);
            if (saveResult.IsFailed)
            {
                _logger.Error("Failed to delete preset in slot {SlotNumber}: {Errors}", 
                    slotNumber, string.Join(", ", saveResult.Errors));
                return Result.Fail($"Failed to delete preset: {saveResult.Errors.First().Message}");
            }

            _logger.Information("Successfully deleted preset in slot {SlotNumber}", slotNumber);
            return Result.Ok();
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Unexpected error deleting preset in slot {SlotNumber}", slotNumber);
            return Result.Fail($"Unexpected error: {ex.Message}");
        }
    }

    /// <summary>
    /// Creates a minimal init preset SysEx with all parameters at default values (0).
    /// </summary>
    private static byte[] CreateInitPresetSysEx(int slotNumber)
    {
        var sysex = new byte[521];
        
        // SysEx header
        sysex[0] = 0xF0; // SysEx start
        sysex[1] = 0x00; // TC Electronic ID
        sysex[2] = 0x20;
        sysex[3] = 0x1F;
        sysex[4] = 0x00; // Device ID
        sysex[5] = 0x63; // Nova System
        sysex[6] = 0x20; // Preset dump
        sysex[7] = 0x01; // Data type: Preset
        sysex[8] = (byte)slotNumber; // Slot number (1-60)
        
        // Reserved byte (void)
        sysex[9] = 0x00;
        
        // Preset name at bytes 9-32 (24 chars): "Init 01", "Init 02", etc.
        var name = $"Init {slotNumber:D2}".PadRight(24);
        var nameBytes = System.Text.Encoding.ASCII.GetBytes(name);
        Array.Copy(nameBytes, 0, sysex, 9, 24);

        // Set minimum valid parameter values (validated ranges)
        Encode4ByteValue(sysex, 38, 500);   // TapTempo: 500ms (100-3000)
        Encode4ByteValue(sysex, 86, 15);    // CompRelease: 15 (13-23)
        Encode4ByteValue(sysex, 330, 50);   // ReverbDecay: 50 (1-200)
        Encode4ByteValue(sysex, 418, 8);    // EqWidth1: 8 (5-12)
        Encode4ByteValue(sysex, 430, 8);    // EqWidth2: 8 (5-12)
        Encode4ByteValue(sysex, 442, 8);    // EqWidth3: 8 (5-12)

        // Calculate checksum (sum of bytes 34-517 & 0x7F)
        int checksum = 0;
        for (int i = 34; i <= 517; i++)
        {
            checksum += sysex[i];
        }
        sysex[518] = (byte)(checksum & 0x7F);
        
        sysex[520] = 0xF7; // SysEx end

        return sysex;
    }

    /// <summary>
    /// Encodes a 4-byte value into SysEx format (7-bit nibbles).
    /// </summary>
    private static void Encode4ByteValue(byte[] sysex, int offset, int value)
    {
        sysex[offset] = (byte)(value & 0x7F);
        sysex[offset + 1] = (byte)((value >> 7) & 0x7F);
        sysex[offset + 2] = (byte)((value >> 14) & 0x7F);
        sysex[offset + 3] = (byte)((value >> 21) & 0x7F);
    }
}
