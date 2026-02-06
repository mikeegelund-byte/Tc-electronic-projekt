namespace Nova.Domain.Models;

/// <summary>
/// Represents a preset mapped to an outgoing MIDI Program Change number.
/// </summary>
/// <param name="PresetNumber">Preset number (31-90)</param>
/// <param name="OutgoingProgram">Outgoing PC number (0-127)</param>
public record ProgramMapOutEntry(int PresetNumber, int OutgoingProgram);
