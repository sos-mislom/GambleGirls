using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GameProject.Core;

internal class Grade
{
    public Texture2D Texture { get; set; }
    public Vector2 Position { get; set; }
    public bool IsActive { get; set; }
    public float Alpha { get; set; }

    public Grade(Texture2D texture, Vector2 position)
    {
        Texture = texture;
        Position = position;
        IsActive = true;
        Alpha = 1.0f; 
    }

    public void Update()
    {
        Alpha -= 0.05f;

        if (Alpha <= 0)
        {
            IsActive = false;
        }
    }
}
