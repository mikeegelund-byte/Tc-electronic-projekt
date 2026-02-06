using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Platform.Storage;

namespace Nova.Presentation.Services;

public static class FileDialogService
{
    public static readonly FilePickerFileType SyxFileType = new("SysEx Files")
    {
        Patterns = new List<string> { "*.syx" }
    };

    public static readonly FilePickerFileType TextFileType = new("Text Files")
    {
        Patterns = new List<string> { "*.txt" }
    };

    public static readonly FilePickerFileType AllFilesType = new("All Files")
    {
        Patterns = new List<string> { "*.*" }
    };

    public static async Task<string?> PickSaveFileAsync(
        string title,
        string suggestedFileName,
        IEnumerable<FilePickerFileType> fileTypes,
        string? defaultExtension = null)
    {
        var topLevel = GetTopLevel();
        if (topLevel?.StorageProvider == null)
            return null;

        var options = new FilePickerSaveOptions
        {
            Title = title,
            SuggestedFileName = suggestedFileName,
            DefaultExtension = defaultExtension,
            FileTypeChoices = fileTypes.ToList()
        };

        var file = await topLevel.StorageProvider.SaveFilePickerAsync(options);
        return file?.Path.LocalPath;
    }

    public static async Task<string?> PickOpenFileAsync(
        string title,
        IEnumerable<FilePickerFileType> fileTypes)
    {
        var topLevel = GetTopLevel();
        if (topLevel?.StorageProvider == null)
            return null;

        var options = new FilePickerOpenOptions
        {
            Title = title,
            AllowMultiple = false,
            FileTypeFilter = fileTypes.ToList()
        };

        var files = await topLevel.StorageProvider.OpenFilePickerAsync(options);
        return files.Count > 0 ? files[0].Path.LocalPath : null;
    }

    private static TopLevel? GetTopLevel()
    {
        if (Avalonia.Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            return desktop.MainWindow;
        }

        return null;
    }
}
