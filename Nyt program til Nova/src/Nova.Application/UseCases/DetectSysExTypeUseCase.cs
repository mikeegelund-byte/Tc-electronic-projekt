using FluentResults;

namespace Nova.Application.UseCases;

/// <summary>
/// Use case for detecting the type of a .syx file by analyzing its contents.
/// Implements Task 8.2.1: Auto-Detect .syx Type.
/// </summary>
public class DetectSysExTypeUseCase : IDetectSysExTypeUseCase
{
    public async Task<Result<SysExType>> ExecuteAsync(string filePath)
    {
        try
        {
            if (!File.Exists(filePath))
                return Result.Fail<SysExType>("File does not exist");

            var data = await File.ReadAllBytesAsync(filePath);

            // Validate minimum SysEx structure
            if (data.Length < 8 || data[0] != 0xF0)
                return Result.Ok(SysExType.Unknown);

            // Check TC Electronic manufacturer ID (00 20 1F) and Nova System model (0x63)
            if (data.Length < 6 || 
                data[1] != 0x00 || data[2] != 0x20 || data[3] != 0x1F || 
                data[5] != 0x63)
                return Result.Ok(SysExType.Unknown);

            // Byte 6 should be 0x20 (Dump message)
            if (data[6] != 0x20)
                return Result.Ok(SysExType.Unknown);

            // Byte 7 determines the data type
            var dataType = data[7];

            // Single preset: 521 bytes, data type 0x01
            if (data.Length == 521 && dataType == 0x01)
                return Result.Ok(SysExType.Preset);

            // System dump: 527 bytes, data type 0x02
            if (data.Length == 527 && dataType == 0x02)
                return Result.Ok(SysExType.SystemDump);

            // User bank: Multiple of 521 bytes (concatenated presets)
            if (data.Length >= 521 && data.Length % 521 == 0 && dataType == 0x01)
                return Result.Ok(SysExType.UserBank);

            return Result.Ok(SysExType.Unknown);
        }
        catch (Exception ex)
        {
            return Result.Fail<SysExType>($"Failed to read file: {ex.Message}");
        }
    }
}
