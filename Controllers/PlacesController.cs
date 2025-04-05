using Microsoft.AspNetCore.Mvc;
using System.Net;
using PublicTransportPlannerApi.Models;
using PublicTransportPlannerApi.Services.GoogleMapsService;

namespace PublicTransportPlannerApi.Controllers;

/// <summary>
/// Controller for Google Places API operations
/// </summary>
[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
public class PlacesController(IGoogleMapsService googleMapsService, ILogger<PlacesController> logger) : ControllerBase
{
    private readonly IGoogleMapsService _googleMapsService = googleMapsService ?? throw new ArgumentNullException(nameof(googleMapsService));
    private readonly ILogger<PlacesController> _logger = logger ?? throw new ArgumentNullException(nameof(logger));


    /// <summary>
    /// Gets place predictions for an input string using Google Places Autocomplete API
    /// </summary>
    /// <param name="request">Place autocomplete request with input text</param>
    /// <returns>List of place predictions</returns>
    /// <response code="200">Returns the place predictions</response>
    /// <response code="400">If the request is invalid</response>
    /// <response code="500">If an error occurs during processing</response>
    [HttpGet("autocomplete")]
    [ProducesResponseType(typeof(PlacesAutocompleteResponse), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.InternalServerError)]
    public async Task<ActionResult<PlacesAutocompleteResponse>> GetPlacePredictionsAsync([FromQuery] PlacesAutocompleteRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);

        if (string.IsNullOrWhiteSpace(request.Input))
        {
            return BadRequest(CreateProblemDetails("Input text cannot be null or empty", HttpStatusCode.BadRequest));
        }

        try
        {
            _logger.LogInformation("Processing place autocomplete request for input: '{Input}'", request.Input);

            PlacesAutocompleteResponse response = await _googleMapsService.GetPlacesAutocompleteAsync(
                request.Input,
                request.Components,
                request.Language);

            _logger.LogInformation("Place autocomplete request processed successfully with {ResultCount} predictions",
                response.Predictions.Count);

            return Ok(response);
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Invalid request: {Message}", ex.Message);
            return BadRequest(CreateProblemDetails(ex.Message, HttpStatusCode.BadRequest));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred during place autocomplete request");
            return StatusCode((int)HttpStatusCode.InternalServerError,
                CreateProblemDetails(ex.Message, HttpStatusCode.InternalServerError));
        }
    }

    /// <summary>
    /// Creates a ProblemDetails object with the given message and status code
    /// </summary>
    /// <param name="message">Error message</param>
    /// <param name="statusCode">HTTP status code</param>
    /// <returns>ProblemDetails object</returns>
    private static ProblemDetails CreateProblemDetails(string message, HttpStatusCode statusCode) =>
        new()
        {
            Title = statusCode == HttpStatusCode.BadRequest ? "Invalid Request" : "Internal Server Error",
            Detail = message,
            Status = (int)statusCode
        };
}
