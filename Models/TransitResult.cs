namespace PublicTransportPlannerApi.Models;

/// <summary>
/// Represents the result of a transit calculation between two addresses
/// </summary>
public record TransitResult
{
    /// <summary>
    /// The starting address
    /// </summary>
    public string Origin { get; init; } = string.Empty;
    
    /// <summary>
    /// The destination address
    /// </summary>
    public string Destination { get; init; } = string.Empty;
    
    /// <summary>
    /// The human-readable duration of the transit (e.g., "1 hour 30 mins")
    /// </summary>
    public string Duration { get; init; } = "N/A";
    
    /// <summary>
    /// The ISO-formatted start date and time of the transit
    /// </summary>
    public string StartDateTime { get; init; } = string.Empty;
    
    /// <summary>
    /// The ISO-formatted arrival date and time at the destination
    /// </summary>
    public string ArrivalDateTime { get; init; } = string.Empty;
    
    /// <summary>
    /// The duration of stay at the destination in seconds, or null if not applicable
    /// </summary>
    public string? StayTime { get; init; }
    
    /// <summary>
    /// The status of the transit calculation (e.g., "OK", "NOT_FOUND", etc.)
    /// </summary>
    public string Status { get; init; } = string.Empty;
}
