namespace GameProject.Tests;
using FluentAssertions;
using Xunit;

public class RhythmSceneTests
{
    [Theory]
    [InlineData(5, 10, 117.9, 55)]
    [InlineData(8, 20, 120.0, 48)]
    [InlineData(0, 15, 100.0, 0)]
    public void CalculationTest(int score, int beatsCount, double songTempo, int expectedScore)
    {
        // Arrange
        
        // Act
        var percentage = (double) score / beatsCount;
        var finalScore = (int)Math.Ceiling(percentage * 10) * ((int) songTempo / 10);

        // Assert
        finalScore.Should().Be(expectedScore);
    }
}