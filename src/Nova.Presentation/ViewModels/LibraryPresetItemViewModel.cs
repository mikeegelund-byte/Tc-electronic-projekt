using Nova.Application.Library;

namespace Nova.Presentation.ViewModels;

public sealed class LibraryPresetItemViewModel
{
    public LibraryPresetItemViewModel(LibraryPresetMetadata metadata)
    {
        Id = metadata.Id;
        Name = metadata.Name;
        Tags = metadata.Tags;
        SourcePresetNumber = metadata.SourcePresetNumber;
        UpdatedAtUtc = metadata.UpdatedAtUtc;
    }

    public Guid Id { get; }
    public string Name { get; }
    public IReadOnlyList<string> Tags { get; }
    public int? SourcePresetNumber { get; }
    public DateTime UpdatedAtUtc { get; }

    public string TagsDisplay => Tags.Count == 0 ? "" : string.Join(", ", Tags);
}
