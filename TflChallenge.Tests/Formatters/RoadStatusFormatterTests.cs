using AutoFixture.Xunit2;
using FluentAssertions;
using TfLChallenge.Formatters;
using TfLChallenge.Models;

namespace TflChallenge.Tests.Unit.Formatters;
public class PlainTextRoadStatusFormatterTests
{
    private readonly PlainTextRoadStatusFormatter _sut;

    public PlainTextRoadStatusFormatterTests()
    {
        _sut = new PlainTextRoadStatusFormatter();
    }

    [Theory, AutoData]
    public void Format_ValidRoadWithDescription_ReturnsFormattedString(RoadStatus status)
    {
        // Act
        var result = _sut.Format(status);

        // Assert
        result.Should().Be($"The status of the {status.DisplayName} is as follows" +
            $"{Environment.NewLine}\tRoad Status is {status.Severity}" +
            $"{Environment.NewLine}\tRoad Status Description is {status.SeverityDescription}");
    }

    [Fact]
    public void Format_NullRoad_ReturnsEmptyString()
    {
        // Act
        var result = _sut.Format(null);

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public void FormatInvalidRoadMsg_RoadIdProvided_ReturnsExpectedMessage()
    {
        // Arrange
        var roadId = "A999";

        // Act
        var result = _sut.FormatInvalidRoadMsg(roadId);

        // Assert
        result.Should().Be("A999 is not a valid road.");
    }
}