namespace Nova.Domain.Models;

/// <summary>
/// Represents a fixed MIDI CC assignment slot (Tap/Drive/etc.) with an optional CC number.
/// </summary>
/// <param name="Assignment">Human-readable assignment name</param>
/// <param name="CCNumber">MIDI CC number (0-127) or null for Off</param>
public record CCMapping(string Assignment, int? CCNumber)
{
    /// <summary>
    /// Indicates whether this slot has an assigned CC number.
    /// </summary>
    public bool IsAssigned => CCNumber.HasValue;
}
