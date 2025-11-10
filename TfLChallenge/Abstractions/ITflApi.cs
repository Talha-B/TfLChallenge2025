using Refit;
using TfLChallenge.Models.TflApiResponses;

namespace TfLChallenge.Abstractions;

public interface ITflApi
{
    [Get("/road/{roadId}")]
    Task<List<RoadStatusResponse>> GetRoadStatusAsync(string roadId);
}
