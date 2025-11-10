using TfLChallenge.Models;

namespace TfLChallenge.Abstractions;

public interface IRoadStatusService
{
    public Task<RoadStatusResult> GetRoadStatus(string roadId);
}