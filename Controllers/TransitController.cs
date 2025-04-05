using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Net;
using PublicTransportPlannerApi.Models;
using PublicTransportPlannerApi.Services.TransitService;

namespace PublicTransportPlannerApi.Controllers;

/// <summary>
/// Controller for handling public transit planning calculations
/// </summary>
/// <remarks>
/// Initializes a new instance of the TransitController
/// </remarks>
/// <param name="transitService">Service for transit calculations</param>
/// <param name="logger">Logger instance</param>
[ApiController]
[Route("api/[controller]")]
public class TransitController(ITransitService transitService, ILogger<TransitController> logger) : ControllerBase
{
    private readonly ITransitService _transitService = transitService ?? throw new ArgumentNullException(nameof(transitService));
    private readonly ILogger<TransitController> _logger = logger ?? throw new ArgumentNullException(nameof(logger));

    /// <summary>
    /// Calculates transit times between multiple addresses
    /// </summary>
    /// <param name="request">Transit calculation request containing addresses and start time</param>
    /// <returns>List of transit results</returns>
    /// <response code="200">Returns the transit calculation results</response>
    /// <response code="400">If the request is invalid</response>
    /// <response code="500">If an error occurs during processing</response>
    [HttpPost("calculate")]
    [ProducesResponseType(typeof(List<TransitResult>), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.InternalServerError)]
    public async Task<ActionResult<List<TransitResult>>> CalculateTransitTimesAsync([Required] TransitCalculationRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);

        if (request.Addresses.Count < 2)
        {
            return BadRequest(CreateProblemDetails("At least two addresses are required", HttpStatusCode.BadRequest));
        }

        try
        {
            _logger.LogInformation("Processing transit calculation request with {AddressCount} addresses", request.Addresses.Count);

            List<TransitResult> results = await _transitService.CalculateTransitTimesAsync(request.Addresses, request.StartTime);

            _logger.LogInformation("Transit calculation request processed successfully with {ResultCount} results", results.Count);
            return Ok(results);
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Invalid request: {Message}", ex.Message);
            return BadRequest(CreateProblemDetails(ex.Message, HttpStatusCode.BadRequest));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred during transit calculation");
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
