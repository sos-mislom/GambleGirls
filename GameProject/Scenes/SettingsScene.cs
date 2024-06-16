using System;
using System.ComponentModel;
using System.IO;
using GameProject.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace GameProject.Scenes;

internal class SettingsScene : Components
{    
    private static readonly String Prefix = "Settings";
    private Texture2D _backgroundTexture;
    private Texture2D _backButtonTexture;
    
    private Button _backButton;

    internal override void LoadContent(ContentManager content)
    {
        _backButtonTexture = content.Load<Texture2D>(Path.Combine(Prefix, "Back"));
        
        _backgroundTexture = content.Load<Texture2D>(Path.Combine(Prefix, "Settings"));
        
        _backButton = new Button(_backButtonTexture,
            new Rectangle(0, 0,
                _backButtonTexture.Width, _backButtonTexture.Height),
            content.Load<SoundEffect>("ButtonHoverSound"));
    }

    internal override void Update(GameTime gameTime)
    {
        _backButton.Update(gameTime);
        HandleButtonClicks();
    }

    internal override void Draw(SpriteBatch spriteBatch)
    {
        spriteBatch.Draw(_backgroundTexture,
            new Rectangle(0, 0, Data.ScreenW, Data.ScreenH),
            Color.White);
        
        _backButton.Draw(spriteBatch);
    }
    
    private void HandleButtonClicks()
    {
        if (Data.MouseState.LeftButton == ButtonState.Pressed &&
            Data.OldMouseState.LeftButton == ButtonState.Released)
        {
            
            if (_backButton.Rectangle.Contains(Data.MouseState.Position))
                Data.CurrentState = Core.Scenes.Start;
        }
    }
}