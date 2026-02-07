using FluentResults;
using Nova.Domain.Models;

namespace Nova.Application.Library;

public interface ILibraryRepository
{
    Task<IReadOnlyList<LibraryPresetMetadata>> ListAsync(CancellationToken cancellationToken);
    Task<LibraryPresetMetadata?> GetMetadataAsync(Guid id, CancellationToken cancellationToken);
    Task<Result<LibraryPresetMetadata>> SavePresetAsync(Preset preset, string? nameOverride, IReadOnlyList<string> tags, CancellationToken cancellationToken);
    Task<Result<Preset>> LoadPresetAsync(Guid id, CancellationToken cancellationToken);
    Task<Result> UpdateMetadataAsync(LibraryPresetMetadata metadata, CancellationToken cancellationToken);
    Task<Result> DeletePresetAsync(Guid id, CancellationToken cancellationToken);
}
