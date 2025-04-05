using System.Text.Json.Serialization;

namespace PublicTransportPlannerApi.Models;

/// <summary>
/// Response model for Google Places Autocomplete API
/// </summary>
public class PlacesAutocompleteResponse
{
    /// <summary>
    /// List of place predictions matching the input query
    /// </summary>
    [JsonPropertyName("predictions")]
    public List<PlacePrediction> Predictions { get; set; } = new();

    /// <summary>
    /// Status of the API response (e.g., "OK", "ZERO_RESULTS", "INVALID_REQUEST")
    /// </summary>
    [JsonPropertyName("status")]
    public string Status { get; set; } = string.Empty;
}

/// <summary>
/// Represents a place prediction from Google Places Autocomplete API
/// </summary>
public class PlacePrediction
{
    /// <summary>
    /// Description of the place
    /// </summary>
    [JsonPropertyName("description")]
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Unique identifier for the place
    /// </summary>
    [JsonPropertyName("place_id")]
    public string PlaceId { get; set; } = string.Empty;

    /// <summary>
    /// Structured formatting of the place description
    /// </summary>
    [JsonPropertyName("structured_formatting")]
    public StructuredFormatting StructuredFormatting { get; set; } = new();

    /// <summary>
    /// Types associated with the place
    /// </summary>
    [JsonPropertyName("types")]
    public List<string> Types { get; set; } = new();
}

/// <summary>
/// Structured formatting of a place description
/// </summary>
public class StructuredFormatting
{
    /// <summary>
    /// Main text portion of the description
    /// </summary>
    [JsonPropertyName("main_text")]
    public string MainText { get; set; } = string.Empty;

    /// <summary>
    /// Secondary text portion of the description
    /// </summary>
    [JsonPropertyName("secondary_text")]
    public string SecondaryText { get; set; } = string.Empty;
}
