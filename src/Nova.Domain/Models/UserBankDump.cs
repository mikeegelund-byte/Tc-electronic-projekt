using FluentResults;

namespace Nova.Domain.Models;

/// <summary>
/// Represents the User Bank of the Nova System pedal (60 presets, numbers 31-90).
/// This class is immutable - use WithPreset() to create modified copies.
/// </summary>
public class UserBankDump
{
    /// <summary>
    /// Array of 60 presets. Index 0 = preset 31, index 59 = preset 90.
    /// Null entries represent empty slots.
    /// </summary>
    public Preset?[] Presets { get; private init; }

    private UserBankDump()
    {
        Presets = new Preset?[60];
    }

    /// <summary>
    /// Creates an empty User Bank with 60 null slots.
    /// </summary>
    public static UserBankDump Empty()
    {
        return new UserBankDump();
    }

    /// <summary>
    /// Creates a new UserBankDump with the specified preset set at the correct index.
    /// </summary>
    /// <param name="presetNumber">Preset number (31-90)</param>
    /// <param name="preset">The preset to set</param>
    /// <returns>Result with new UserBankDump or error</returns>
    public Result<UserBankDump> WithPreset(int presetNumber, Preset preset)
    {
        if (presetNumber < 31 || presetNumber > 90)
            return Result.Fail($"Preset number must be between 31 and 90, got {presetNumber}");

        if (preset.Number != presetNumber)
            return Result.Fail($"Preset number mismatch: trying to set preset {preset.Number} at position {presetNumber}");

        var newPresets = new Preset?[60];
        Array.Copy(Presets, newPresets, 60);
        newPresets[presetNumber - 31] = preset;

        return Result.Ok(new UserBankDump { Presets = newPresets });
    }

    /// <summary>
    /// Creates a UserBankDump from a collection of presets.
    /// </summary>
    /// <param name="presets">Collection of 60 presets numbered 31-90</param>
    /// <returns>Result with UserBankDump or error</returns>
    public static Result<UserBankDump> FromPresets(IEnumerable<Preset> presets)
    {
        var presetList = presets.ToList();

        if (presetList.Count != 60)
            return Result.Fail($"Expected 60 presets, got {presetList.Count}");

        var bank = Empty();
        foreach (var preset in presetList)
        {
            var result = bank.WithPreset(preset.Number, preset);
            if (result.IsFailed)
                return result;
            bank = result.Value;
        }

        return Result.Ok(bank);
    }
}
