using AutoFixture.Xunit2;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using Refit;
using System.Net;
using TfLChallenge.Abstractions;
using TfLChallenge.Enums;
using TfLChallenge.Models;
using TfLChallenge.Models.TflApiResponses;
using TfLChallenge.Services;

namespace TflChallenge.Tests.Unit.Services;
public class RoadStatusServiceTests
{
    private readonly ITflApi _tflApi;
    private readonly IRoadStatusFormatter _formatter;
    private readonly ILogger<RoadStatusService> _logger;
    private readonly RoadStatusService _sut;

    public RoadStatusServiceTests()
    {
        _tflApi = Substitute.For<ITflApi>();
        _formatter = Substitute.For<IRoadStatusFormatter>();
        _logger = Substitute.For<ILogger<RoadStatusService>>();
        _sut = new RoadStatusService(_tflApi, _formatter, _logger);
    }

    [Theory, AutoData]
    public async Task GetRoadStatus_ValidRoad_ReturnsSuccess(string roadId, RoadStatusResponse roadStatus, string formattedOutput)
    {
        // Arrange
        _tflApi.GetRoadStatusAsync(roadId).Returns([roadStatus]);
        _formatter.Format(Arg.Any<RoadStatus>()).Returns(formattedOutput);

        // Act
        var result = await _sut.GetRoadStatus(roadId);

        // Assert
        result.StatusCode.Should().Be(RoadStatusCode.Success);
        result.Output.Should().Be(formattedOutput);
    }

    [Theory, AutoData]
    public async Task GetRoadStatus_NoDataFromApi_ReturnsError(string roadId)
    {
        // Arrange
        _tflApi.GetRoadStatusAsync(roadId).Returns([]);

        // Act
        var result = await _sut.GetRoadStatus(roadId);

        // Assert
        result.StatusCode.Should().Be(RoadStatusCode.Error);
        result.Output.Should().Contain($"No data returned for road '{roadId}'.");
    }

    [Theory, AutoData]
    public async Task GetRoadStatus_RoadNotFound_ReturnsNotFound(string roadId, string formattedError)
    {
        // Arrange
        var requestMessage = new HttpRequestMessage(HttpMethod.Get, $"https://api.tfl.gov.uk/road/{roadId}");
        var apiException = await ApiException.Create(requestMessage, HttpMethod.Get, new HttpResponseMessage(HttpStatusCode.NotFound), new());

        _tflApi.GetRoadStatusAsync(roadId).ThrowsAsync(apiException);
        _formatter.FormatInvalidRoadMsg(roadId).Returns(formattedError);

        // Act
        var result = await _sut.GetRoadStatus(roadId);

        // Assert
        result.StatusCode.Should().Be(RoadStatusCode.NotFound);
        result.Output.Should().Be(formattedError);
    }

    [Theory, AutoData]
    public async Task GetRoadStatus_UnexpectedException_ReturnsError(string roadId)
    {
        // Arrange
        var exception = new InvalidOperationException("Error");

        _tflApi.When(x => x.GetRoadStatusAsync(roadId)).Do(_ => throw exception);

        // Act
        var result = await _sut.GetRoadStatus(roadId);

        // Assert
        result.StatusCode.Should().Be(RoadStatusCode.Error);
        result.Output.Should().Contain($"An unexpected error occurred fetching road {roadId}.");
    }
}