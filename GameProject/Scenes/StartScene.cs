using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using GameProject.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace GameProject.Scenes;

internal class StartScene : Components
{
    private static readonly String Prefix = "StartScene";
    
    private Song _backgroundMusic;
    private Texture2D _backgroundTexture;

    private static readonly string[] Names = {
        Path.Combine(Prefix, "SettingsButton"),
        Path.Combine(Prefix, "StartButton"),
        Path.Combine(Prefix, "ExitButton")};

    private Button[] _buttons = new Button[Names.Length];
    private List<FlyingImage> _flyingGifs = new();
    
    private readonly Random _random = new();
    
    private TimeSpan _idleTime = TimeSpan.Zero;
    private static readonly TimeSpan IdleTimeout = TimeSpan.FromSeconds(3);
    
    private Texture2D _logoTexture;
    private Texture2D _songName;
    private Texture2D _gameLabel;
    
    private Vector2 _logoPosition;
    private float _logoScale = 1f;
    
    private List<float> _beats;
    private float _songTime;
    private float _tempo;
    
    private int _currentBeatIndex;
    
    internal override void LoadContent(ContentManager content)
    {
        LoadAssets(content);
        LoadFlyingGifs(content);
        PlayBackgroundMusic();
        LoadButtons(content);
    }
    internal override void Update(GameTime gameTime)
    {
        UpdateButtons(gameTime);
        
        HandleButtonClicks();
        UpdateLogoScale(gameTime);
        UpdateFlyingGifs(gameTime);
        CheckIdleState(gameTime);
        
        if (Data.CurrentState == Core.Scenes.Start 
            && MediaPlayer.State != MediaState.Playing)
            PlayBackgroundMusic();
    }
    
    internal override void Draw(SpriteBatch spriteBatch)
    {
        DrawBackground(spriteBatch);
        DrawFlyingGifs(spriteBatch);
        DrawLogoOrButtons(spriteBatch);
        DrawText(spriteBatch);
    }
    
    private void LoadAssets(ContentManager content)
    {
        
        _logoTexture = content.Load<Texture2D>(Path.Combine(Prefix, "Logo"));
        _gameLabel = content.Load<Texture2D>(Path.Combine(Prefix, "GameLabel"));
        _backgroundTexture = content.Load<Texture2D>(Path.Combine(Prefix, "BackgroundImage"));
        _backgroundTexture = content.Load<Texture2D>(Path.Combine(Prefix, "BackgroundImage"));
        
        //var startGSong = Girl.GetRandomSong(0, _random);
        var startGSong = Data.Girls[0].Songs[0];
        
        Data.CurrentSongModel = startGSong;
        _backgroundMusic = startGSong.Song;
        _songName = startGSong.Name;
        
        _tempo = startGSong.SongDataModel.tempo;
        _beats = startGSong.SongDataModel.beats;
    }
    
    
    private void LoadButtons(ContentManager content)
    {
        for (var i = 0; i < _buttons.Length; i++)
        {
            var buttonTexture = content.Load<Texture2D>(Names[i]);
            
            var buttonWidth = buttonTexture.Width;
            var buttonHeight = buttonTexture.Height;
            
            var buttonX = 45;
            if (i == 2)
                buttonX = Data.ScreenW - buttonWidth - 45;
            if (i == 1)
                buttonX = (Data.ScreenW - buttonWidth) / 2;
            
            var buttonRect = new Rectangle(buttonX, Data.ScreenH-buttonHeight+50, buttonWidth, buttonHeight);
            _buttons[i] = new Button(buttonTexture, buttonRect, 
                content.Load<SoundEffect>("ButtonHoverSound"));
        }
    }



    private void LoadFlyingGifs(ContentManager content)
    {
        const int gifCount = 20;

        for (var i = 0; i < gifCount; i++)
        {
            Texture2D randomGif = content.Load<Texture2D>(Path.Combine(Prefix, "Partical") + (_random.Next(1, 4)));
            _flyingGifs.Add(new FlyingImage(randomGif,
                new Vector2(_random.Next(0, Data.ScreenW), Data.ScreenW + randomGif.Height),
                new Vector2(0, -_random.Next(1, 5))));
        }
    }

    private void PlayBackgroundMusic()
    {
        MediaPlayer.Play(_backgroundMusic);
        MediaPlayer.IsRepeating = true;
    }

    private void UpdateButtons(GameTime gameTime)
    {
        foreach (var button in _buttons)
            button.Update(gameTime);
    }

    private void CheckIdleState(GameTime gameTime)
    {
        if (Data.MouseState != Data.OldMouseState)
        {
            _idleTime = TimeSpan.Zero;
            foreach (var button in _buttons)
                button.Visible = true;
        }
        else
        {
            _idleTime += gameTime.ElapsedGameTime;
        
            if (_idleTime >= IdleTimeout)
            {
                foreach (var button in _buttons)
                    button.Visible = false;
            }
        }
    }


    private void HandleButtonClicks()
    {
        if (Data.MouseState.LeftButton == ButtonState.Pressed)
        {
            if (_buttons[1].Rectangle.Contains(Data.MouseState.Position))
            {
                Data.CurrentState = Core.Scenes.Main;
                MediaPlayer.Stop();
            }
            else if (_buttons[2].Rectangle.Contains(Data.MouseState.Position))
            {
                Data.Exit = true;
                MediaPlayer.Stop();
            }
            else if (_buttons[0].Rectangle.Contains(Data.MouseState.Position))
            {
                Data.CurrentState = Core.Scenes.Settings;
                MediaPlayer.Stop();
            }
        }
    }

    private void UpdateLogoScale(GameTime gameTime)
    {
        if (_beats?.Count > 0)
        {
            _songTime += (float) gameTime.ElapsedGameTime.TotalSeconds;
            if (_currentBeatIndex < _beats.Count && _songTime >= _beats[_currentBeatIndex])
            {
                _logoScale = 1.2f;
                _currentBeatIndex++;
            }
            else
            {
                _logoScale = Math.Max(1f, _logoScale - 0.01f);
            }
        }
        else
        {
            _logoScale = 1 + ((float)Math.Cos(gameTime.TotalGameTime.TotalSeconds * 2) +
                              (float)Math.Sin(gameTime.TotalGameTime.TotalSeconds * 4)) * 0.1f
                           + ((float)_random.NextDouble() - 0.5f) * 0.1f;
        }
    }


    private void UpdateFlyingGifs(GameTime gameTime)
    {
        for (int i = _flyingGifs.Count - 1; i >= 0; i--)
        {
            _flyingGifs[i].Update(gameTime);

            if (_flyingGifs[i].Position.Y + _flyingGifs[i].Texture.Height < 0)
            {
                _flyingGifs[i].Position = new Vector2(_random.Next(0, Data.ScreenW),
                    Data.ScreenH + _flyingGifs[i].Texture.Height);
                
                _flyingGifs[i].Velocity = new Vector2(0, -_random.Next(1, 5));
            }
        }
    }

    private void DrawBackground(SpriteBatch spriteBatch)
    {
        spriteBatch.Draw(_backgroundTexture,
            new Rectangle(0, 0, Data.ScreenW, Data.ScreenH),
            Color.White);
    }

    private void DrawFlyingGifs(SpriteBatch spriteBatch)
    {
        foreach (var gif in _flyingGifs)
        {
            var origin = new Vector2(gif.Texture.Width / 2, gif.Texture.Height / 2);
            spriteBatch.Draw(gif.Texture, gif.Position, null, Color.White, 0f, origin, gif.Scale, SpriteEffects.None, 0f);
        }
    }
    private void DrawText(SpriteBatch spriteBatch)
    {
        if (_buttons[0].Visible)
        {
            spriteBatch.Draw(_gameLabel,
                new Rectangle(45,45,_gameLabel.Width, _gameLabel.Height),
                Color.White);
       
            spriteBatch.Draw(_songName,
                new Rectangle(45, _gameLabel.Height + 45 ,_songName.Width, _songName.Height),
                Color.White); 
        }
        
        if (!_buttons[0].Visible)
            spriteBatch.Draw(_songName,
                new Rectangle(45, 45 ,_songName.Width, _songName.Height),
                Color.White);
        
    }
    private void DrawLogoOrButtons(SpriteBatch spriteBatch)
    {

        if (!_buttons[0].Visible)
        {
            _logoPosition = new Vector2(
                Data.ScreenW - (_logoTexture.Width)-250, 
                Data.ScreenH - (_logoTexture.Width)-150);
            
            var logoOrigin = new Vector2(_logoTexture.Width / 2, _logoTexture.Height / 2);
            
            spriteBatch.Draw(_logoTexture, _logoPosition,
                null, Color.White, 0f, logoOrigin,
                _logoScale, SpriteEffects.None, 0f);
        }
        else
        {
            _logoPosition = new Vector2((Data.ScreenW - _logoTexture.Width + 100), 200);
            var logoOrigin = new Vector2(_logoTexture.Width / 2, _logoTexture.Height / 2);

            spriteBatch.Draw(_logoTexture, _logoPosition, null, 
                Color.White, 0f, logoOrigin, _logoScale, SpriteEffects.None, 0f);

            foreach (var button in _buttons)
            {
                if (button.Visible)
                    button.Draw(spriteBatch);
            }
        }
    }
    
}
