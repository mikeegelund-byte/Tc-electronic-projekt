namespace Nova.Application.Library;

public sealed record LibraryPresetMetadata(
    Guid Id,
    string Name,
    IReadOnlyList<string> Tags,
    DateTime CreatedAtUtc,
    DateTime UpdatedAtUtc,
    int? SourcePresetNumber);
