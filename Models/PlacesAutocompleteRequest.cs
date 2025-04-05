using System.ComponentModel.DataAnnotations;

namespace PublicTransportPlannerApi.Models;

/// <summary>
/// Request model for Place Autocomplete API
/// </summary>
public class PlacesAutocompleteRequest
{
    /// <summary>
    /// The text string on which to search
    /// </summary>
    [Required]
    public string Input { get; set; } = string.Empty;

    /// <summary>
    /// The component filters separated by | used to restrict results to a specific country
    /// Default is "country:at" for Austria
    /// </summary>
    public string Components { get; set; } = "country:at";

    /// <summary>
    /// The language code specifying the language of the results
    /// Default is "de" for German
    /// </summary>
    public string Language { get; set; } = "de";
}
