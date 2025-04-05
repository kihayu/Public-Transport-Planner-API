using PublicTransportPlannerApi.Models;

namespace PublicTransportPlannerApi.Services.GoogleMapsService;

/// <summary>
/// Interface for Google Maps service operations
/// </summary>
public interface IGoogleMapsService
{
    /// <summary>
    /// Gets distance matrix data between two locations using transit mode
    /// </summary>
    /// <param name="origin">The origin address</param>
    /// <param name="destination">The destination address</param>
    /// <param name="departureTime">UNIX timestamp for departure time</param>
    /// <returns>DistanceMatrixResponse with transit information</returns>
    Task<DistanceMatrixResponse> GetDistanceMatrixAsync(string origin, string destination, long departureTime);

    /// <summary>
    /// Gets place predictions for an input string using Google Places Autocomplete API
    /// </summary>
    /// <param name="input">The input text to search for places</param>
    /// <param name="components">The component filters (e.g., country:at)</param>
    /// <param name="language">The language for the results (e.g., de)</param>
    /// <returns>PlacesAutocompleteResponse with place predictions</returns>
    /// <exception cref="ArgumentNullException">Thrown when input is null or empty</exception>
    /// <exception cref="InvalidOperationException">Thrown when response cannot be processed</exception>
    /// <exception cref="HttpRequestException">Thrown when HTTP request fails</exception>
    Task<PlacesAutocompleteResponse> GetPlacesAutocompleteAsync(string input, string components, string language);
}
