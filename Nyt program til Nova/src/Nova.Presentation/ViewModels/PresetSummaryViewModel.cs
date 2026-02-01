namespace Nova.Presentation.ViewModels;

using Nova.Domain.Models;

/// <summary>
/// Display model for a single preset in the list view.
/// Maps from Preset domain model to UI-friendly properties.
/// </summary>
public record PresetSummaryViewModel(
    int Number,
    string Position,  // "00-1", "00-2", "00-3", "01-1", etc.
    string Name,
    int BankGroup     // 0-19
)
{
    /// <summary>
    /// Converts a Preset domain model to a display ViewModel.
    /// Nova System presets are numbered 31-90 (60 presets).
    /// Bank groups: 0-19 (20 banks of 3 presets each).
    /// Handles edge cases: Empty or whitespace names display as "[Unnamed #XX]".
    /// </summary>
    public static PresetSummaryViewModel FromPreset(Preset preset)
    {
        // Calculate bank group (0-19) and slot (1-3)
        var bankGroup = (preset.Number - 31) / 3;
        var slot = ((preset.Number - 31) % 3) + 1;
        var position = $"{bankGroup:D2}-{slot}";
        
        // Handle empty or corrupt names - Task 2.5 edge case handling
        var displayName = string.IsNullOrWhiteSpace(preset.Name)
            ? $"[Unnamed #{preset.Number}]"
            : preset.Name.Trim();
        
        return new PresetSummaryViewModel(
            preset.Number,
            position,
            displayName,
            bankGroup
        );
    }
}
