namespace Nova.Domain.Models;

/// <summary>
/// Represents a MIDI CC (Control Change) to parameter mapping.
/// </summary>
/// <param name="CCNumber">MIDI CC number (0-127), or 0xFF for unassigned</param>
/// <param name="ParameterId">Parameter ID being controlled, or 0xFF for unassigned</param>
public record CCMapping(byte CCNumber, byte ParameterId)
{
    /// <summary>
    /// Indicates whether this CC mapping slot is assigned.
    /// </summary>
    public bool IsAssigned => CCNumber != 0xFF && ParameterId != 0xFF;
}
