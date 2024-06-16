using System;
using System.Collections.Generic;
using GameProject.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace GameProject.Scenes
{
    internal class SongsScene : Components
    {
        private Texture2D _backgroundTexture;
        private Texture2D _backButtonTexture;
        private Button _backButton;
        private List<SoundButton> _songButtons;
        private List<Button> _previewTextures;
        
        private int _girlId;

        public void SetGirlId(int girlId)
        {
            _girlId = girlId;
        }

        internal override void LoadContent(ContentManager content)
        {
            _backgroundTexture = Data.Girls[_girlId].BackgroundImage;
            _backButtonTexture = Data.Girls[_girlId].BackButton;

            _backButton = new Button(_backButtonTexture,
                new Rectangle(0, -10, _backButtonTexture.Width, _backButtonTexture.Height), 
                content.Load<SoundEffect>("ButtonHoverSound"));
            
            _songButtons = new List<SoundButton>();
            _previewTextures = new List<Button>();
            
            var buttonSpacing = 40; 
            var totalHeight = _backButtonTexture.Height; 

            for (var i = 0; i < Data.Girls[_girlId].Songs.Count; i++)
            {
                var song = Data.Girls[_girlId].Songs[i];
                
                int buttonY = totalHeight + buttonSpacing * i;
                
                _previewTextures.Add(new Button(song.Preview, 
                    new Rectangle(45, buttonY, song.Preview.Width, song.Preview.Height),
                    null));
                
                _songButtons.Add(new SoundButton(song.Name,
                    new Rectangle(song.Preview.Width + 45, buttonY, song.Name.Width, song.Name.Height), song));
                
                totalHeight += song.Name.Height;
            }
        }

        internal override void Update(GameTime gameTime)
        {
            if (Data.MouseState.LeftButton == ButtonState.Pressed &&
                Data.OldMouseState.LeftButton == ButtonState.Released)
            {
                if (_backButton.Rectangle.Contains(Data.MouseState.Position))
                    Data.CurrentState = Core.Scenes.Girl;
                
                foreach (var songButton in _songButtons)
                {
                    if (songButton.Rectangle.Contains(Data.MouseState.Position))
                    {
                        Data.CurrentState = Core.Scenes.RhythmGame;
                        MediaPlayer.Stop();
                    }

                }
            }
            
            _backButton.Update(gameTime);
            
            foreach (var songButton in _songButtons)
                songButton.Update(gameTime);
            
        }

        internal override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(_backgroundTexture, new Rectangle(0, 0, Data.ScreenW, Data.ScreenH), Color.White);

            _backButton.Draw(spriteBatch);

            foreach (var songButton in _songButtons)
                songButton.Draw(spriteBatch);
            
            foreach (var preview in _previewTextures)
                preview.Draw(spriteBatch);
        }
    }
}
