using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Refit;
using System.Net;
using TfLChallenge.Abstractions;
using TfLChallenge.Enums;
using TfLChallenge.Formatters;
using TfLChallenge.Services;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.Server;

namespace TflChallenge.Tests.Integration;

public class RoadStatusServiceTests : IAsyncLifetime
{
    private WireMockServer _mockServer;
    private IRoadStatusService _sut;

    public async Task InitializeAsync()
    {
        _mockServer = WireMockServer.Start();

        var api = RestService.For<ITflApi>(_mockServer.Url);

        _sut = new RoadStatusService(api, new PlainTextRoadStatusFormatter(), NullLogger<RoadStatusService>.Instance);

        await Task.CompletedTask;
    }

    [Fact]
    public async Task GetRoadStatus_ValidRoad_ReturnsSuccess()
    {
        // Arrange
        const string roadId = "A2";
        SetupRoadResponse(roadId, HttpStatusCode.OK, @"
            [
                {
                    ""id"": ""A2"",
                    ""displayName"": ""A2"",
                    ""statusSeverity"": ""Good"",
                    ""statusSeverityDescription"": ""No Exceptional Delays"",
                    ""bounds"": ""[[-0.0857,51.44091],[0.17118,51.49438]]"",
                    ""envelope"": ""[[-0.0857,51.44091],[-0.0857,51.49438],[0.17118,51.49438],[0.17118,51.44091],[-0.0857,51.44091]]"",
                    ""url"": ""/Road/a2""
                }
            ]");

        // Act
        var result = await _sut.GetRoadStatus(roadId);

        // Assert
        result.StatusCode.Should().Be(RoadStatusCode.Success);
        result.Output.Should().Contain("The status of the A2 is as follows");
        result.Output.Should().Contain("Road Status is Good");
        result.Output.Should().Contain("Road Status Description is No Exceptional Delays");
    }

    [Fact]
    public async Task GetRoadStatus_InvalidRoad_ReturnsNotFound()
    {
        // Arrange
        const string roadId = "invalid_road";
        SetupRoadResponse(roadId, HttpStatusCode.NotFound);

        // Act
        var result = await _sut.GetRoadStatus(roadId);

        // Assert
        result.StatusCode.Should().Be(RoadStatusCode.NotFound);
        result.Output.Should().Contain($"{roadId} is not a valid road.");
    }

    private void SetupRoadResponse(string roadId, HttpStatusCode statusCode, string body = null)
    {
        var responseBuilder = Response.Create().WithStatusCode(statusCode);

        if (!string.IsNullOrEmpty(body))
        {
            responseBuilder = responseBuilder
                .WithHeader("Content-Type", "application/json")
                .WithBody(body);
        }

        _mockServer.Given(Request.Create().WithPath($"/road/{roadId}").UsingGet())
               .RespondWith(responseBuilder);
    }

    public Task DisposeAsync()
    {
        _mockServer.Stop();
        _mockServer.Dispose();
        return Task.CompletedTask;
    }
}