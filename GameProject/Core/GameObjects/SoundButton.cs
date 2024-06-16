using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace GameProject.Core;

internal class SoundButton(Texture2D texture, Rectangle rectangle, SongModel songModel) : Components
{
    private bool _isHovered;
    private readonly int _originalWidth = rectangle.Width;
    private readonly int _originalHeight = rectangle.Height;

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
            Data.CurrentSongModel = songModel;
            
            MediaPlayer.Play(songModel.Song);
            rectangle.Width += 10; 
            rectangle.Height += 10; 
        }
        else if (!rectangle.Contains(Data.MouseState.Position)
                 && rectangle.Contains(Data.OldMouseState.Position))
        {
            MediaPlayer.Stop();
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