using TfLChallenge.Abstractions;
using TfLChallenge.Enums;

namespace TfLChallenge;

public class App(IRoadStatusService roadStatusService)
{
    private readonly IRoadStatusService _roadStatusService = roadStatusService;

    public async Task Run()
    {
        Console.Write("Please enter a road: ");

        var road = Console.ReadLine();
        var result = await _roadStatusService.GetRoadStatus(road);

        Console.WriteLine(result.Output);
        Environment.Exit(result.StatusCode == RoadStatusCode.Success ? 0 : 1);
    }
}