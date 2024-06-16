using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace GameProject.Core;

public class FlyingImage(Texture2D texture, Vector2 position, Vector2 velocity) : Components
{
    public Texture2D Texture { get; private set; } = texture;
    public Vector2 Position { get; set; } = position;
    public Vector2 Velocity { get; set; } = velocity;
    public float Scale { get; private set; } = 1f;

    internal override void LoadContent(ContentManager content)
    {
        throw new NotImplementedException();
    }

    internal override void Update(GameTime gameTime)
    {
        Position += Velocity;
        Scale = 1 + (float)Math.Sin(gameTime.TotalGameTime.TotalSeconds * 3) * 0.2f;
    }
    
    internal override void Draw(SpriteBatch spriteBatch)
    {
        throw new NotImplementedException();
    }
    
    public void UpdateTest(GameTime gameTime)
    {
        Position += Velocity;
        Scale = 1 + (float)Math.Sin(gameTime.TotalGameTime.TotalSeconds * 3) * 0.2f;
    }
}