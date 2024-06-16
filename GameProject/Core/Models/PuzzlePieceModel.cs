using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GameProject.Core;

internal class PuzzlePieceModel
{
    public Texture2D Texture { get; }
    public Vector2 Position { get; set; }
    public Vector2 TargetPosition { get; }
    public bool IsLocked { get; set; }
    public bool IsVisible { get; set; }
    public Rectangle Bounds => new Rectangle((int)Position.X, (int)Position.Y, Texture.Width, Texture.Height);

    public PuzzlePieceModel(Texture2D texture, Vector2 position, Vector2 targetPosition, bool isLocked, bool isVisible)
    {
        Texture = texture;
        Position = position;
        TargetPosition = targetPosition;
        IsLocked = isLocked;
        IsVisible = isVisible;
    }
}