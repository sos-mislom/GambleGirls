using System;
using GameProject.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Xunit;

namespace GameProject.Tests
{
    public class FlyingImageTests
    {
        [Fact]
        public void Update_PositionIsCorrect()
        {
            // Arrange
            var position = new Vector2(100, 100);
            var velocity = new Vector2(2, 2);
            var flyingImage = new FlyingImage(null, position, velocity);
            var gameTime = new GameTime(TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(1));

            // Act
            flyingImage.UpdateTest(gameTime);

            // Assert
            Assert.Equal(new Vector2(102, 102), flyingImage.Position); 
        }

        [Fact]
        public void Update_ScaleIsCorrect()
        {
            // Arrange
            var position = new Vector2(100, 100);
            var velocity = new Vector2(2, 2);
            var flyingImage = new FlyingImage(null, position, velocity);
            var gameTime = new GameTime(TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(1));

            // Act
            flyingImage.UpdateTest(gameTime);

            // Assert
            Assert.InRange(flyingImage.Scale, 0.8f, 1.2f); 
        }
    }
}