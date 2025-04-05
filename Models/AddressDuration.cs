namespace PublicTransportPlannerApi.Models;

/// <summary>
/// Represents an address with an associated duration to stay at that location
/// </summary>
public record AddressDuration
{
    /// <summary>
    /// The address as a string (e.g., "123 Main St, City, Country")
    /// </summary>
    public required string Address { get; init; }
    
    /// <summary>
    /// The duration in seconds to stay at this location
    /// </summary>
    public int Duration { get; init; }
}
