using GameProject.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;

namespace GameProject.Tests;
using FluentAssertions;
using Xunit;

public class StartSceneTests
{
    [Theory]
    [InlineData(0, 600, 45, 550)] 
    [InlineData(800, 0, 45, -50)] 
    [InlineData(1600, 1000, 45, 950)] 
    public void ButtonPosition_FirstButton_PositionIsCorrect(int screenW, int screenH, int expectedX, int expectedY)
    {
        // Arrange
        var buttons = new Button[3];
        var buttonWidth = 100;
        var buttonHeight = 100;

        var names = new string[] { "Button1", "Button2", "Button3" };

        // Act
        for (var i = 0; i < buttons.Length; i++)
        {
            var buttonX = 45;
            if (i == 2)
                buttonX = screenW - buttonWidth - 45;
            if (i == 1)
                buttonX = (screenW - buttonWidth) / 2;

            var buttonRect = new Rectangle(buttonX, screenH - buttonHeight + 50, buttonWidth, buttonHeight);
            buttons[i] = new Button(null, buttonRect, null);
        }

        // Assert
        buttons[0].Rectangle.X.Should().Be(expectedX);
        buttons[0].Rectangle.Y.Should().Be(expectedY);
    }
}