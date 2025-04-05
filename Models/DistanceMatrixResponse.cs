using System.Text.Json.Serialization;

namespace PublicTransportPlannerApi.Models;

/// <summary>
/// Represents the response from Google Maps Distance Matrix API
/// </summary>
public record DistanceMatrixResponse
{
    /// <summary>
    /// Status of the API response
    /// </summary>
    [JsonPropertyName("status")]
    public string Status { get; init; } = string.Empty;

    /// <summary>
    /// Origin addresses
    /// </summary>
    [JsonPropertyName("origin_addresses")]
    public string[] OriginAddresses { get; init; } = [];

    /// <summary>
    /// Destination addresses
    /// </summary>
    [JsonPropertyName("destination_addresses")]
    public string[] DestinationAddresses { get; init; } = [];

    /// <summary>
    /// Rows containing distance and duration information
    /// </summary>
    [JsonPropertyName("rows")]
    public Row[] Rows { get; init; } = [];
}

/// <summary>
/// Represents a row in the distance matrix response
/// </summary>
public record Row
{
    /// <summary>
    /// Elements containing distance and duration information
    /// </summary>
    [JsonPropertyName("elements")]
    public Element[] Elements { get; init; } = [];
}

/// <summary>
/// Represents an element in a row
/// </summary>
public record Element
{
    /// <summary>
    /// Status of the element
    /// </summary>
    [JsonPropertyName("status")]
    public string Status { get; init; } = string.Empty;

    /// <summary>
    /// Duration information
    /// </summary>
    [JsonPropertyName("duration")]
    public Duration? Duration { get; init; }

    /// <summary>
    /// Distance information
    /// </summary>
    [JsonPropertyName("distance")]
    public Distance? Distance { get; init; }
}

/// <summary>
/// Represents duration information
/// </summary>
public record Duration
{
    /// <summary>
    /// Human-readable text representation of the duration
    /// </summary>
    [JsonPropertyName("text")]
    public string Text { get; init; } = string.Empty;

    /// <summary>
    /// Duration value in seconds
    /// </summary>
    [JsonPropertyName("value")]
    public int Value { get; init; }
}

/// <summary>
/// Represents distance information
/// </summary>
public record Distance
{
    /// <summary>
    /// Human-readable text representation of the distance
    /// </summary>
    [JsonPropertyName("text")]
    public string Text { get; init; } = string.Empty;

    /// <summary>
    /// Distance value in meters
    /// </summary>
    [JsonPropertyName("value")]
    public int Value { get; init; }
}
