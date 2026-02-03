using FluentResults;
using Nova.Domain.Models;

namespace Nova.Application.UseCases;

/// <summary>
/// Use case interface for requesting a System Dump from the hardware.
/// </summary>
public interface IRequestSystemDumpUseCase
{
    /// <summary>
    /// Requests system dump from Nova System pedal.
    /// </summary>
    /// <param name="timeoutMs">Timeout in milliseconds for receiving response</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Result containing parsed SystemDump or error details</returns>
    Task<Result<SystemDump>> ExecuteAsync(int timeoutMs = 5000, CancellationToken cancellationToken = default);
}
