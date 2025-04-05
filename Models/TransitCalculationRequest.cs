namespace PublicTransportPlannerApi.Models;

/// <summary>
/// Request model for transit time calculation
/// </summary>
public record TransitCalculationRequest
{
    /// <summary>
    /// List of addresses with durations to stay at each location
    /// </summary>
    public required List<AddressDuration> Addresses { get; init; }
    
    /// <summary>
    /// Unix timestamp representing the start time for the first transit
    /// </summary>
    public required long StartTime { get; init; }
}
