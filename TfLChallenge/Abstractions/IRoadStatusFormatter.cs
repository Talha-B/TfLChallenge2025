using TfLChallenge.Models;

namespace TfLChallenge.Abstractions;

public interface IRoadStatusFormatter
{
    public string Format(RoadStatus status);

    public string FormatInvalidRoadMsg(string roadId);
}