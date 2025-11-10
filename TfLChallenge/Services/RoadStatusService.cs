using Microsoft.Extensions.Logging;
using Refit;
using TfLChallenge.Abstractions;
using TfLChallenge.Enums;
using TfLChallenge.Models;

namespace TfLChallenge.Services;

public class RoadStatusService(ITflApi api, IRoadStatusFormatter formatter, ILogger<RoadStatusService> logger) : IRoadStatusService
{
    private readonly ITflApi _api = api;
    private readonly IRoadStatusFormatter _formatter = formatter;
    private readonly ILogger<RoadStatusService> _logger = logger;

    public async Task<RoadStatusResult> GetRoadStatus(string roadId)
    {
        if (string.IsNullOrWhiteSpace(roadId))
        {
            return new RoadStatusResult(RoadStatusCode.InvalidInput, "RoadId cannot be null or empty.");
        }

        roadId = roadId.Trim();

        try
        {
            var results = await _api.GetRoadStatusAsync(roadId);
            var roadStatusResponse = results?.FirstOrDefault();

            if (string.IsNullOrWhiteSpace(roadStatusResponse?.StatusSeverity))
            {
                return new RoadStatusResult(RoadStatusCode.Error, $"No data returned for road '{roadId}'.");
            }

            var roadStatus = new RoadStatus(roadStatusResponse.DisplayName, roadStatusResponse.StatusSeverity, roadStatusResponse.StatusSeverityDescription);

            return new RoadStatusResult(RoadStatusCode.Success, _formatter.Format(roadStatus));
        }
        catch (ApiException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            return new RoadStatusResult(RoadStatusCode.NotFound, _formatter.FormatInvalidRoadMsg(roadId));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error fetching road status for {RoadId}", roadId);
            return new RoadStatusResult(RoadStatusCode.Error, $"An unexpected error occurred fetching road {roadId}.");
        }
    }
}