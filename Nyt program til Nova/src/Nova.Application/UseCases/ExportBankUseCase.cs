using FluentResults;
using Nova.Domain.Models;

namespace Nova.Application.UseCases;

/// <summary>
/// Use case for exporting the entire User Bank (60 presets) to a .syx file.
/// </summary>
public class ExportBankUseCase : IExportBankUseCase
{
    public async Task<Result> ExecuteAsync(UserBankDump bank, string filePath)
    {
        // Filter out null presets (empty slots)
        var presets = bank.Presets.Where(p => p != null).ToList();

        if (presets.Count == 0)
            return Result.Fail("No presets found in bank to export");

        try
        {
            // Concatenate all preset SysEx messages
            using var stream = new MemoryStream();
            foreach (var preset in presets)
            {
                await stream.WriteAsync(preset!.RawSysEx);
            }

            // Write concatenated bytes to file
            var allBytes = stream.ToArray();
            await File.WriteAllBytesAsync(filePath, allBytes);

            return Result.Ok();
        }
        catch (Exception ex)
        {
            return Result.Fail($"Failed to write bank file: {ex.Message}");
        }
    }
}
