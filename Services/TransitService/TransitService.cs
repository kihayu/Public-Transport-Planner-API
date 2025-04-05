using PublicTransportPlannerApi.Models;
using PublicTransportPlannerApi.Services.GoogleMapsService;

namespace PublicTransportPlannerApi.Services.TransitService;

/// <summary>
/// Service implementation for transit calculations
/// </summary>
/// <remarks>
/// Initializes a new instance of the TransitService
/// </remarks>
/// <param name="googleMapsService">Google Maps service for API operations</param>
/// <param name="logger">Logger instance</param>
public class TransitService(IGoogleMapsService googleMapsService, ILogger<TransitService> logger) : ITransitService
{
    private const string StatusOk = "OK";
    private const string Iso8601Format = "o";
    private const int MinimumAddressCount = 2;

    private readonly IGoogleMapsService _googleMapsService = googleMapsService ?? throw new ArgumentNullException(nameof(googleMapsService));
    private readonly ILogger<TransitService> _logger = logger ?? throw new ArgumentNullException(nameof(logger));

    /// <summary>
    /// Calculates transit times between multiple addresses
    /// </summary>
    /// <param name="addresses">List of addresses with durations</param>
    /// <param name="startTime">Start time as UNIX timestamp</param>
    /// <returns>List of transit results</returns>
    /// <exception cref="ArgumentException">Thrown when input data is invalid</exception>
    /// <exception cref="ArgumentNullException">Thrown when addresses is null</exception>
    /// <exception cref="InvalidOperationException">Thrown when API response is invalid</exception>
    public async Task<List<TransitResult>> CalculateTransitTimesAsync(List<AddressDuration> addresses, long startTime)
    {
        // Validate input
        ArgumentNullException.ThrowIfNull(addresses);

        if (addresses.Count < MinimumAddressCount)
        {
            throw new ArgumentException($"At least {MinimumAddressCount} addresses are required", nameof(addresses));
        }

        _logger.LogInformation("Calculating transit times for {AddressCount} addresses", addresses.Count);

        var results = new List<TransitResult>();
        long departureTime = startTime;

        for (int i = 0; i < addresses.Count - 1; i++)
        {
            var (Result, NextDepartureTime) = await CalculateSegmentTransitAsync(addresses, i, departureTime);
            results.Add(Result);
            departureTime = NextDepartureTime;
        }

        _logger.LogInformation("Transit calculation completed successfully with {ResultCount} results", results.Count);
        return results;
    }

    /// <summary>
    /// Calculates transit information for a single segment between two addresses
    /// </summary>
    /// <param name="addresses">List of addresses with durations</param>
    /// <param name="segmentIndex">Index of the current segment</param>
    /// <param name="departureTime">Departure time as UNIX timestamp</param>
    /// <returns>Transit result and next departure time</returns>
    /// <exception cref="InvalidOperationException">Thrown when API response is invalid</exception>
    private async Task<(TransitResult Result, long NextDepartureTime)> CalculateSegmentTransitAsync(
        List<AddressDuration> addresses, int segmentIndex, long departureTime)
    {
        var origin = addresses[segmentIndex];
        var destination = addresses[segmentIndex + 1];

        try
        {
            var response = await _googleMapsService.GetDistanceMatrixAsync(
                origin.Address,
                destination.Address,
                departureTime);

            ValidateResponse(response, segmentIndex);
            var element = response.Rows[0].Elements[0];
            ValidateElement(element, segmentIndex);

            var result = CreateTransitResult(origin, destination, element, departureTime);
            long nextDepartureTime = CalculateNextDepartureTime(departureTime, destination.Duration, element.Duration!.Value);

            return (result, nextDepartureTime);
        }
        catch (Exception ex) when (ex is not ArgumentException and not InvalidOperationException)
        {
            _logger.LogError(ex, "Error calculating distance for segment {Index}", segmentIndex);
            throw new InvalidOperationException($"Error calculating distance for segment {segmentIndex}", ex);
        }
    }

    /// <summary>
    /// Validates the response from the Google Maps API
    /// </summary>
    /// <param name="response">Distance matrix response</param>
    /// <param name="segmentIndex">Index of the current segment</param>
    /// <exception cref="InvalidOperationException">Thrown when response is invalid</exception>
    private void ValidateResponse(DistanceMatrixResponse response, int segmentIndex)
    {
        if (response.Status != StatusOk || response.Rows.Length == 0 || response.Rows[0].Elements.Length == 0)
        {
            _logger.LogWarning("Invalid response for segment {Index}: {Status}", segmentIndex, response.Status);
            throw new InvalidOperationException($"Failed to calculate distance: {response.Status}");
        }
    }

    /// <summary>
    /// Validates the element from the Google Maps API response
    /// </summary>
    /// <param name="element">Response element</param>
    /// <param name="segmentIndex">Index of the current segment</param>
    /// <exception cref="InvalidOperationException">Thrown when element is invalid</exception>
    private void ValidateElement(Element element, int segmentIndex)
    {
        if (element.Status != StatusOk || element.Duration == null)
        {
            _logger.LogWarning("Failed to calculate distance for segment {Index}: {Status}",
                segmentIndex, element.Status);
            throw new InvalidOperationException($"Failed to calculate distance: {element.Status}");
        }
    }

    /// <summary>
    /// Creates a transit result object from the API response data
    /// </summary>
    /// <param name="origin">Origin address with duration</param>
    /// <param name="destination">Destination address with duration</param>
    /// <param name="element">Response element</param>
    /// <param name="departureTime">Departure time as UNIX timestamp</param>
    /// <returns>Transit result</returns>
    private static TransitResult CreateTransitResult(
        AddressDuration origin, AddressDuration destination, Element element, long departureTime) =>
        new()
        {
            Origin = origin.Address,
            Destination = destination.Address,
            Duration = element.Duration!.Text,
            StartDateTime = DateTimeOffset.FromUnixTimeSeconds(departureTime).ToString(Iso8601Format),
            ArrivalDateTime = DateTimeOffset.FromUnixTimeSeconds(departureTime + element.Duration.Value).ToString(Iso8601Format),
            StayTime = destination.Duration != 0 ? destination.Duration.ToString() : null,
            Status = element.Status
        };

    /// <summary>
    /// Calculates the next departure time based on the current transit information
    /// </summary>
    /// <param name="currentDepartureTime">Current departure time</param>
    /// <param name="stayDuration">Stay duration in seconds</param>
    /// <param name="transitDuration">Transit duration in seconds</param>
    /// <returns>Next departure time as UNIX timestamp</returns>
    private static long CalculateNextDepartureTime(long currentDepartureTime, int stayDuration, int transitDuration) =>
        currentDepartureTime + stayDuration + transitDuration;
}
