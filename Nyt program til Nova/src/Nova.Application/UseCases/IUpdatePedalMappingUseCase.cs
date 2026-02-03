namespace Nova.Application.UseCases;

/// <summary>
/// Use case for updating expression pedal mapping in SystemDump and saving to hardware.
/// </summary>
public interface IUpdatePedalMappingUseCase
{
    /// <summary>
    /// Updates the expression pedal mapping and saves to hardware.
    /// </summary>
    /// <param name="parameter">Parameter ID (0-127)</param>
    /// <param name="min">Minimum value (0-100%)</param>
    /// <param name="mid">Midpoint value (0-100%)</param>
    /// <param name="max">Maximum value (0-100%)</param>
    /// <returns>Result indicating success or failure</returns>
    Task<FluentResults.Result> UpdateAsync(int parameter, int min, int mid, int max);
}
