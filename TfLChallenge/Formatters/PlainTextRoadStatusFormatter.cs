using TfLChallenge.Abstractions;
using TfLChallenge.Models;

namespace TfLChallenge.Formatters;

public class PlainTextRoadStatusFormatter : IRoadStatusFormatter
{
    public string Format(RoadStatus status)
    {
        if (status is null)
        {
            return string.Empty;
        }

        var output = new List<string>
        {
            $"The status of the {status.DisplayName} is as follows",
            $"Road Status is {status.Severity}",
            $"Road Status Description is {status.SeverityDescription}"
        };

        return string.Join(Environment.NewLine + "\t", output);
    }

    public string FormatInvalidRoadMsg(string roadId) => $"{roadId} is not a valid road.";
}