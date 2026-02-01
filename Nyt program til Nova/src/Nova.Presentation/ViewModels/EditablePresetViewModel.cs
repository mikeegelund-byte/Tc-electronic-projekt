using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FluentResults;
using Microsoft.Extensions.Logging;
using Nova.Application.UseCases;
using Nova.Domain.Models;
using Nova.Midi;
using System.ComponentModel.DataAnnotations;

namespace Nova.Presentation.ViewModels;

/// <summary>
/// ViewModel for editing a preset with validation and change tracking.
/// Provides observable properties, commands, and validation for preset editing.
/// </summary>
public partial class EditablePresetViewModel : ObservableValidator
{
    private readonly UpdatePresetUseCase _updatePresetUseCase;
    private readonly IMidiPort _midiPort;
    private readonly ILogger<EditablePresetViewModel>? _logger;
    private Preset? _originalPreset;
    private bool _isLoading;

    // Observable properties for basic preset info
    [ObservableProperty]
    private int _presetNumber;

    [ObservableProperty]
    [Required(ErrorMessage = "Preset name is required")]
    [StringLength(24, ErrorMessage = "Preset name cannot exceed 24 characters")]
    [MinLength(1, ErrorMessage = "Preset name cannot be empty")]
    [NotifyDataErrorInfo]
    [NotifyPropertyChangedFor(nameof(HasChanges))]
    private string _presetName = string.Empty;

    // Global parameters
    [ObservableProperty]
    [Range(0, 100, ErrorMessage = "Volume must be between 0 and 100")]
    [NotifyDataErrorInfo]
    [NotifyPropertyChangedFor(nameof(HasChanges))]
    private int _volume;

    [ObservableProperty]
    [Range(-50, 50, ErrorMessage = "Pan must be between -50 (left) and 50 (right)")]
    [NotifyDataErrorInfo]
    [NotifyPropertyChangedFor(nameof(HasChanges))]
    private int _pan;

    [ObservableProperty]
    [Range(-12, 12, ErrorMessage = "Transpose must be between -12 and 12 semitones")]
    [NotifyDataErrorInfo]
    [NotifyPropertyChangedFor(nameof(HasChanges))]
    private int _transpose;

    [ObservableProperty]
    [Range(-2, 2, ErrorMessage = "Octave must be between -2 and 2")]
    [NotifyDataErrorInfo]
    [NotifyPropertyChangedFor(nameof(HasChanges))]
    private int _octave;

    // Additional observable properties for UI state
    [ObservableProperty]
    private bool _isSaving;

    [ObservableProperty]
    private string _statusMessage = string.Empty;

    public EditablePresetViewModel(
        UpdatePresetUseCase updatePresetUseCase,
        IMidiPort midiPort,
        ILogger<EditablePresetViewModel>? logger = null)
    {
        _updatePresetUseCase = updatePresetUseCase;
        _midiPort = midiPort;
        _logger = logger;
    }

    /// <summary>
    /// Indicates whether any changes have been made to the preset.
    /// </summary>
    public bool HasChanges
    {
        get
        {
            if (_isLoading || _originalPreset == null)
                return false;

            return PresetName != _originalPreset.Name;
            // Note: We only track preset name changes for now as the requirement specifies
            // basic editable properties. Volume, Pan, Transpose, Octave are placeholders
            // for future implementation when the Preset model supports setting these values.
        }
    }

    /// <summary>
    /// Loads a preset for editing.
    /// </summary>
    public void LoadPreset(Preset preset)
    {
        if (preset == null)
            throw new ArgumentNullException(nameof(preset));

        _isLoading = true;
        _originalPreset = preset;

        PresetNumber = preset.Number;
        PresetName = preset.Name;
        
        // Note: The current Preset model doesn't expose direct Volume/Pan/Transpose/Octave properties
        // These would need to be mapped from the appropriate effect parameters
        // For now, we set them to defaults
        Volume = 100;
        Pan = 0;
        Transpose = 0;
        Octave = 0;

        StatusMessage = string.Empty;
        _isLoading = false;
        OnPropertyChanged(nameof(HasChanges));
    }

    /// <summary>
    /// Saves the modified preset to the pedal.
    /// </summary>
    [RelayCommand(CanExecute = nameof(CanSave))]
    private async Task SaveAsync(CancellationToken cancellationToken)
    {
        if (_originalPreset == null)
        {
            StatusMessage = "No preset loaded";
            return;
        }

        // Validate all properties
        ValidateAllProperties();
        if (HasErrors)
        {
            StatusMessage = "Please fix validation errors before saving";
            _logger?.LogWarning("Validation errors prevent saving preset");
            return;
        }

        IsSaving = true;
        StatusMessage = "Saving preset...";

        try
        {
            // Create modified preset
            // Note: Since Preset is immutable and doesn't have a builder/With methods,
            // we need to modify the RawSysEx directly
            var modifiedPreset = CreateModifiedPreset();
            if (modifiedPreset == null)
            {
                StatusMessage = "Failed to create modified preset";
                IsSaving = false;
                return;
            }

            // Send to pedal
            var result = await _updatePresetUseCase.ExecuteAsync(
                modifiedPreset,
                _midiPort,
                cancellationToken);

            if (result.IsSuccess)
            {
                StatusMessage = "Preset saved successfully";
                _originalPreset = modifiedPreset;
                OnPropertyChanged(nameof(HasChanges));
                _logger?.LogInformation("Successfully saved preset {PresetNumber}", PresetNumber);
            }
            else
            {
                StatusMessage = $"Save failed: {string.Join(", ", result.Errors.Select(e => e.Message))}";
                _logger?.LogError("Failed to save preset: {Errors}", StatusMessage);
            }
        }
        catch (Exception ex)
        {
            StatusMessage = $"Error: {ex.Message}";
            _logger?.LogError(ex, "Error saving preset {PresetNumber}", PresetNumber);
        }
        finally
        {
            IsSaving = false;
        }
    }

    private bool CanSave() => HasChanges && !IsSaving && _originalPreset != null;

    /// <summary>
    /// Cancels editing and reverts changes.
    /// </summary>
    [RelayCommand]
    private void Cancel()
    {
        if (_originalPreset != null)
        {
            LoadPreset(_originalPreset);
            StatusMessage = "Changes reverted";
            _logger?.LogInformation("User cancelled preset editing for preset {PresetNumber}", PresetNumber);
        }
    }

    /// <summary>
    /// Creates a modified preset with updated name.
    /// </summary>
    private Preset? CreateModifiedPreset()
    {
        if (_originalPreset == null)
            return null;

        // Clone the original SysEx data
        var modifiedSysEx = (byte[])_originalPreset.RawSysEx.Clone();

        // Update preset name (bytes 9-32, 24 characters)
        var nameBytes = System.Text.Encoding.ASCII.GetBytes(PresetName.PadRight(24));
        Array.Copy(nameBytes, 0, modifiedSysEx, 9, Math.Min(nameBytes.Length, 24));

        // Parse back to create new Preset
        var result = Preset.FromSysEx(modifiedSysEx);
        return result.IsSuccess ? result.Value : null;
    }

    partial void OnPresetNameChanged(string value)
    {
        SaveCommand.NotifyCanExecuteChanged();
    }
}
