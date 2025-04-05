using PublicTransportPlannerApi.Models;

namespace PublicTransportPlannerApi.Services.TransitService;

/// <summary>
/// Service for calculating transit times between addresses
/// </summary>
public interface ITransitService
{
    /// <summary>
    /// Calculates transit times between multiple addresses
    /// </summary>
    /// <param name="addresses">List of addresses with durations</param>
    /// <param name="startTime">Start time as UNIX timestamp</param>
    /// <returns>List of transit results</returns>
    Task<List<TransitResult>> CalculateTransitTimesAsync(List<AddressDuration> addresses, long startTime);
}
