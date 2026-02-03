using FluentResults;
using Nova.Domain.Models;

namespace Nova.Application.UseCases;

public class UpdateCCMappingUseCase : IUpdateCCMappingUseCase
{
    public Task<Result> ExecuteAsync(SystemDump systemDump, int ccIndex, int? ccNumber)
    {
        if (systemDump == null)
            return Task.FromResult(Result.Fail("System Dump cannot be null"));

        if (ccIndex < 0 || ccIndex > 10)
            return Task.FromResult(Result.Fail("CC index must be between 0 and 10"));

        if (ccNumber.HasValue && (ccNumber.Value < 0 || ccNumber.Value > 127))
            return Task.FromResult(Result.Fail("CC number must be between 0 and 127"));

        var updateResult = systemDump.UpdateCCMapping(ccIndex, ccNumber);
        return Task.FromResult(updateResult);
    }
}
