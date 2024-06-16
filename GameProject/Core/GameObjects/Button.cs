using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace GameProject.Core;

public class Button(Texture2D texture, Rectangle rectangle, SoundEffect hoverSoundEffect) : Components
{
    private bool _isHovered;
    private readonly int _originalWidth = rectangle.Width;
    private readonly int _originalHeight = rectangle.Height;

    public bool Visible { get; set; }
    public Rectangle Rectangle
    {
        get => rectangle;
        set => rectangle = value;
    }

    internal override void LoadContent(ContentManager content)
    {
        throw new System.NotImplementedException();
    }

    internal override void Update(GameTime gameTime)
    {
        if (rectangle.Contains(Data.MouseState.Position)
            && !rectangle.Contains(Data.OldMouseState.Position))
        {
            _isHovered = true;
            hoverSoundEffect?.Play();
            rectangle.Width += 10; 
            rectangle.Height += 10; 
        }
        else if (!rectangle.Contains(Data.MouseState.Position)
                 && rectangle.Contains(Data.OldMouseState.Position))
        {
            rectangle.Width = _originalWidth;
            rectangle.Height = _originalHeight;
        } else {
            _isHovered = false;
        }
    }

    internal override void Draw(SpriteBatch spriteBatch)
    {
        spriteBatch.Draw(texture, rectangle, _isHovered ? Color.Beige : Color.White);
    }
}
