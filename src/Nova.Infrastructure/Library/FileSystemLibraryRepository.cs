using System.Text.Json;
using FluentResults;
using Nova.Application.Library;
using Nova.Domain.Models;

namespace Nova.Infrastructure.Library;

public sealed class FileSystemLibraryRepository : ILibraryRepository
{
    private readonly string _rootPath;
    private readonly JsonSerializerOptions _jsonOptions = new()
    {
        WriteIndented = true
    };

    public FileSystemLibraryRepository()
    {
        var appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        _rootPath = Path.Combine(appData, "NovaSystemManager", "Library");
        Directory.CreateDirectory(_rootPath);
    }

    public async Task<IReadOnlyList<LibraryPresetMetadata>> ListAsync(CancellationToken cancellationToken)
    {
        var results = new List<LibraryPresetMetadata>();

        foreach (var dir in Directory.EnumerateDirectories(_rootPath))
        {
            cancellationToken.ThrowIfCancellationRequested();

            var metadataPath = Path.Combine(dir, "metadata.json");
            if (!File.Exists(metadataPath))
                continue;

            try
            {
                var json = await File.ReadAllTextAsync(metadataPath, cancellationToken);
                var metadata = JsonSerializer.Deserialize<LibraryPresetMetadata>(json, _jsonOptions);
                if (metadata != null)
                {
                    results.Add(metadata);
                }
            }
            catch
            {
                // Ignore malformed entries
            }
        }

        return results
            .OrderByDescending(m => m.UpdatedAtUtc)
            .ToList();
    }

    public async Task<LibraryPresetMetadata?> GetMetadataAsync(Guid id, CancellationToken cancellationToken)
    {
        var metadataPath = GetMetadataPath(id);
        if (!File.Exists(metadataPath))
            return null;

        var json = await File.ReadAllTextAsync(metadataPath, cancellationToken);
        return JsonSerializer.Deserialize<LibraryPresetMetadata>(json, _jsonOptions);
    }

    public async Task<Result<LibraryPresetMetadata>> SavePresetAsync(
        Preset preset,
        string? nameOverride,
        IReadOnlyList<string> tags,
        CancellationToken cancellationToken)
    {
        try
        {
            var sysexResult = preset.ToSysEx();
            if (sysexResult.IsFailed)
                return Result.Fail(sysexResult.Errors);

            var id = Guid.NewGuid();
            var folder = GetPresetFolder(id);
            Directory.CreateDirectory(folder);

            var normalizedTags = NormalizeTags(tags);
            var now = DateTime.UtcNow;
            var name = string.IsNullOrWhiteSpace(nameOverride) ? preset.Name : nameOverride.Trim();

            var metadata = new LibraryPresetMetadata(
                id,
                name,
                normalizedTags,
                now,
                now,
                preset.Number);

            var presetPath = Path.Combine(folder, "preset.syx");
            var metadataPath = Path.Combine(folder, "metadata.json");

            await File.WriteAllBytesAsync(presetPath, sysexResult.Value, cancellationToken);
            var json = JsonSerializer.Serialize(metadata, _jsonOptions);
            await File.WriteAllTextAsync(metadataPath, json, cancellationToken);

            return Result.Ok(metadata);
        }
        catch (Exception ex)
        {
            return Result.Fail($"Failed to save preset: {ex.Message}");
        }
    }

    public async Task<Result<Preset>> LoadPresetAsync(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            var presetPath = GetPresetPath(id);
            if (!File.Exists(presetPath))
                return Result.Fail("Preset file not found");

            var bytes = await File.ReadAllBytesAsync(presetPath, cancellationToken);
            var presetResult = Preset.FromSysEx(bytes);
            return presetResult.IsSuccess
                ? Result.Ok(presetResult.Value)
                : Result.Fail(presetResult.Errors);
        }
        catch (Exception ex)
        {
            return Result.Fail($"Failed to load preset: {ex.Message}");
        }
    }

    public async Task<Result> UpdateMetadataAsync(LibraryPresetMetadata metadata, CancellationToken cancellationToken)
    {
        try
        {
            var metadataPath = GetMetadataPath(metadata.Id);
            if (!File.Exists(metadataPath))
                return Result.Fail("Metadata not found");

            var updated = metadata with
            {
                UpdatedAtUtc = DateTime.UtcNow,
                Tags = NormalizeTags(metadata.Tags)
            };

            var json = JsonSerializer.Serialize(updated, _jsonOptions);
            await File.WriteAllTextAsync(metadataPath, json, cancellationToken);
            return Result.Ok();
        }
        catch (Exception ex)
        {
            return Result.Fail($"Failed to update metadata: {ex.Message}");
        }
    }

    public Task<Result> DeletePresetAsync(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            var folder = GetPresetFolder(id);
            if (Directory.Exists(folder))
            {
                Directory.Delete(folder, recursive: true);
            }

            return Task.FromResult(Result.Ok());
        }
        catch (Exception ex)
        {
            return Task.FromResult(Result.Fail($"Failed to delete preset: {ex.Message}"));
        }
    }

    private string GetPresetFolder(Guid id) => Path.Combine(_rootPath, id.ToString("N"));
    private string GetPresetPath(Guid id) => Path.Combine(GetPresetFolder(id), "preset.syx");
    private string GetMetadataPath(Guid id) => Path.Combine(GetPresetFolder(id), "metadata.json");

    private static IReadOnlyList<string> NormalizeTags(IEnumerable<string> tags)
    {
        return tags
            .Where(t => !string.IsNullOrWhiteSpace(t))
            .Select(t => t.Trim())
            .Where(t => t.Length > 0)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .OrderBy(t => t)
            .ToList();
    }
}
