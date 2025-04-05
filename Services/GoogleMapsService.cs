using System.Net.Http.Json;
using System.Web;
using Microsoft.Extensions.Configuration;
using PublicTransportPlannerApi.Models;

namespace PublicTransportPlannerApi.Services;

/// <summary>
/// Service for interacting with Google Maps API
/// </summary>
public class GoogleMapsService : IGoogleMapsService
{
    private const string BaseUrl = "https://maps.googleapis.com/";
    private const string TransitMode = "transit";
    private const string DefaultLanguage = "de";
    private const string PlacesApiPath = "maps/api/place/autocomplete/json";
    private const string DistanceMatrixApiPath = "maps/api/distancematrix/json";

    private readonly HttpClient _httpClient;
    private readonly string _apiKey;
    private readonly ILogger<GoogleMapsService> _logger;

    /// <summary>
    /// Initializes a new instance of the GoogleMapsService
    /// </summary>
    /// <param name="httpClient">HTTP client for API requests</param>
    /// <param name="configuration">Application configuration</param>
    /// <param name="logger">Logger instance</param>
    public GoogleMapsService(HttpClient httpClient, IConfiguration configuration, ILogger<GoogleMapsService> logger)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _apiKey = configuration["GoogleMaps:ApiKey"] ?? throw new InvalidOperationException("Google Maps API key is not configured");

        _httpClient.BaseAddress = new Uri(BaseUrl);
    }

    /// <summary>
    /// Gets distance matrix data between two locations using transit mode
    /// </summary>
    /// <param name="origin">The origin address</param>
    /// <param name="destination">The destination address</param>
    /// <param name="departureTime">UNIX timestamp for departure time</param>
    /// <returns>DistanceMatrixResponse with transit information</returns>
    /// <exception cref="ArgumentNullException">Thrown when required parameters are null</exception>
    /// <exception cref="InvalidOperationException">Thrown when response cannot be processed</exception>
    /// <exception cref="HttpRequestException">Thrown when HTTP request fails</exception>
    public async Task<DistanceMatrixResponse> GetDistanceMatrixAsync(string origin, string destination, long departureTime)
    {
        if (string.IsNullOrWhiteSpace(origin))
            throw new ArgumentNullException(nameof(origin));

        if (string.IsNullOrWhiteSpace(destination))
            throw new ArgumentNullException(nameof(destination));

        try
        {
            _logger.LogInformation("Requesting distance matrix data for {Origin} to {Destination}", origin, destination);

            string requestUrl = BuildDistanceMatrixUrl(origin, destination, departureTime);
            var response = await _httpClient.GetAsync(requestUrl);
            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadFromJsonAsync<DistanceMatrixResponse>();

            if (result == null)
            {
                _logger.LogError("Failed to deserialize distance matrix response");
                throw new InvalidOperationException("Failed to deserialize distance matrix response");
            }

            return result;
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "HTTP error occurred while retrieving distance matrix");
            throw;
        }
        catch (Exception ex) when (ex is not ArgumentNullException and not InvalidOperationException)
        {
            _logger.LogError(ex, "Error occurred while retrieving distance matrix");
            throw new InvalidOperationException("Failed to retrieve distance matrix data", ex);
        }
    }

    /// <summary>
    /// Builds the request URL for the distance matrix API
    /// </summary>
    /// <param name="origin">The origin address</param>
    /// <param name="destination">The destination address</param>
    /// <param name="departureTime">UNIX timestamp for departure time</param>
    /// <returns>The formatted request URL</returns>
    private string BuildDistanceMatrixUrl(string origin, string destination, long departureTime)
    {
        return $"{DistanceMatrixApiPath}?{BuildDistanceMatrixQueryString(origin, destination, departureTime)}";
    }

    /// <summary>
    /// Builds the query string for the distance matrix API request
    /// </summary>
    /// <param name="origin">The origin address</param>
    /// <param name="destination">The destination address</param>
    /// <param name="departureTime">UNIX timestamp for departure time</param>
    /// <returns>The formatted query string</returns>
    private string BuildDistanceMatrixQueryString(string origin, string destination, long departureTime)
    {
        var queryParams = new Dictionary<string, string>
        {
            { "origins", origin },
            { "destinations", destination },
            { "mode", TransitMode },
            { "departure_time", departureTime.ToString() },
            { "language", DefaultLanguage },
            { "key", _apiKey }
        };

        return string.Join("&", queryParams.Select(p => $"{HttpUtility.UrlEncode(p.Key)}={HttpUtility.UrlEncode(p.Value)}"));
    }

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
    public async Task<PlacesAutocompleteResponse> GetPlacesAutocompleteAsync(string input, string components, string language)
    {
        if (string.IsNullOrWhiteSpace(input))
            throw new ArgumentNullException(nameof(input));

        try
        {
            _logger.LogInformation("Requesting place predictions for input: '{Input}'", input);

            string requestUrl = BuildPlacesAutocompleteUrl(input, components, language);
            var response = await _httpClient.GetAsync(requestUrl);
            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadFromJsonAsync<PlacesAutocompleteResponse>();

            if (result == null)
            {
                _logger.LogError("Failed to deserialize places autocomplete response");
                throw new InvalidOperationException("Failed to deserialize places autocomplete response");
            }

            return result;
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "HTTP error occurred while retrieving place predictions");
            throw;
        }
        catch (Exception ex) when (ex is not ArgumentNullException and not InvalidOperationException)
        {
            _logger.LogError(ex, "Error occurred while retrieving place predictions");
            throw new InvalidOperationException("Failed to retrieve place predictions", ex);
        }
    }

    /// <summary>
    /// Builds the request URL for the places autocomplete API
    /// </summary>
    /// <param name="input">The input text to search for places</param>
    /// <param name="components">The component filters (e.g., country:at)</param>
    /// <param name="language">The language for the results (e.g., de)</param>
    /// <returns>The formatted request URL</returns>
    private string BuildPlacesAutocompleteUrl(string input, string components, string language)
    {
        return $"{PlacesApiPath}?{BuildPlacesAutocompleteQueryString(input, components, language)}";
    }

    /// <summary>
    /// Builds the query string for the places autocomplete API request
    /// </summary>
    /// <param name="input">The input text to search for places</param>
    /// <param name="components">The component filters (e.g., country:at)</param>
    /// <param name="language">The language for the results (e.g., de)</param>
    /// <returns>The formatted query string</returns>
    private string BuildPlacesAutocompleteQueryString(string input, string components, string language)
    {
        var queryParams = new Dictionary<string, string>
        {
            { "input", input },
            { "components", components },
            { "language", language },
            { "key", _apiKey }
        };

        return string.Join("&", queryParams.Select(p => $"{HttpUtility.UrlEncode(p.Key)}={HttpUtility.UrlEncode(p.Value)}"));
    }
}
