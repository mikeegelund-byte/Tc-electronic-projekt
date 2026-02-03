using FluentResults;
using Nova.Domain.Models;

namespace Nova.Application.UseCases;

public class UpdateCCMappingUseCase : IUpdateCCMappingUseCase
{
    public Task<Result> ExecuteAsync(SystemDump systemDump, int ccIndex, byte ccNumber, byte parameterId)
    {
        if (systemDump == null)
            return Task.FromResult(Result.Fail("System Dump cannot be null"));

        if (ccIndex < 0 || ccIndex > 63)
            return Task.FromResult(Result.Fail("CC index must be between 0 and 63"));

        if (ccNumber > 127 && ccNumber != 0xFF)
            return Task.FromResult(Result.Fail("CC number must be between 0 and 127 or 0xFF for unassigned"));

        var updateResult = systemDump.UpdateCCMapping(ccIndex, ccNumber, parameterId);
        return Task.FromResult(updateResult);
    }
}
