namespace Nova.Domain.Models;

/// <summary>
/// Represents an incoming MIDI Program Change mapping to a preset.
/// </summary>
/// <param name="IncomingProgram">Incoming PC number (1-127)</param>
/// <param name="PresetNumber">Preset number (1-90) or null for none</param>
public record ProgramMapInEntry(int IncomingProgram, int? PresetNumber)
{
    public bool IsAssigned => PresetNumber.HasValue;
}
