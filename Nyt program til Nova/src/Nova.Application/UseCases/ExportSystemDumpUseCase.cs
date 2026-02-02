using FluentResults;
using Nova.Domain.Models;

namespace Nova.Application.UseCases;

/// <summary>
/// Use case for exporting System Settings to a .syx file.
/// </summary>
public class ExportSystemDumpUseCase : IExportSystemDumpUseCase
{
    public async Task<Result> ExecuteAsync(SystemDump systemDump, string filePath)
    {
        try
        {
            // Write SysEx bytes to file
            await File.WriteAllBytesAsync(filePath, systemDump.RawSysEx);
            return Result.Ok();
        }
        catch (Exception ex)
        {
            return Result.Fail($"Failed to write system dump file: {ex.Message}");
        }
    }
}
