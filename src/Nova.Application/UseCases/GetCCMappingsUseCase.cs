using FluentResults;
using Nova.Domain.Models;

namespace Nova.Application.UseCases;

/// <summary>
/// Use case for retrieving all 64 MIDI CC to parameter mappings from a System Dump.
/// </summary>
public class GetCCMappingsUseCase : IGetCCMappingsUseCase
{
    public Task<Result<List<CCMapping>>> ExecuteAsync(SystemDump systemDump)
    {
        if (systemDump == null)
        {
            return Task.FromResult(Result.Fail<List<CCMapping>>("System Dump cannot be null"));
        }

        var result = systemDump.GetAllCCMappings();
        return Task.FromResult(result);
    }
}
