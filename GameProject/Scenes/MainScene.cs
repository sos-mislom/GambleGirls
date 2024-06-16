using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using GameProject.Core;
using GameProject.Managers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace GameProject.Scenes;

internal class MainScene : Components
{
    private static readonly String Prefix = "MainScene";
    
    private readonly Random _random = new();
    
    private Song _backgroundMusic;
    private SoundEffect _hoverSoundEffect;

    private Texture2D _QRTexture;
    private Texture2D _backgroundMusicName;
    private Texture2D _backgroundTexture;
    private Texture2D _movementElementTexture;
    private Texture2D _currentCardTexture;
    private Texture2D _nextCardTexture;
    
    private List<Vector2> _movementElementPositions = new();
    
    private int CurrentCardIndex = 0;
    private float _movementElementXOffset;
    private float _movementElementSpeed = 2f; 

    private Button _leftButton;
    private Button _rightButton;
    private Button _middleButton;
    
    internal override void LoadContent(ContentManager content)
    {
        LoadAssets(content);
        
        _leftButton = new Button(null,
            new Rectangle(0, 0, Data.ScreenW / 2 - 300, Data.ScreenH),
            _hoverSoundEffect);
        
        _rightButton = new Button(null,
            new Rectangle(Data.ScreenW / 2 + 300, 0, Data.ScreenW / 2 - 300, Data.ScreenH),
            _hoverSoundEffect);
        
        _middleButton = new Button(null,
            new Rectangle(Data.ScreenW / 2 - 300, 0, 600, Data.ScreenH),
            null);
    }
    
    internal override void Update(GameTime gameTime)
    {
        _movementElementXOffset = (float)Math.Sin(
            gameTime.TotalGameTime.TotalSeconds * _movementElementSpeed)*2;
        
        UpdateButtons(gameTime);
        HandleInput();
        HandleKeyboard();
        
        if (Data.CurrentState == Core.Scenes.Main 
            && MediaPlayer.State != MediaState.Playing)
            PlayBackgroundMusic();
    }
    
    internal override void Draw(SpriteBatch spriteBatch)
    {
        spriteBatch.Draw(_backgroundTexture,
            new Rectangle(0, 0, Data.ScreenW, Data.ScreenH),
            Color.White);

        DrawCards(spriteBatch);
        
        if (_movementElementTexture != null)
        {
            var movementElementPosition = new Vector2(
                _movementElementPositions[CurrentCardIndex].X + _movementElementXOffset, 
                _movementElementPositions[CurrentCardIndex].Y);

            spriteBatch.Draw(_movementElementTexture, movementElementPosition, null, 
                Color.White, 0f, Vector2.Zero, 0.75f, SpriteEffects.None, 0f);
        }

        DrawInfo(spriteBatch);
    }
    
    private void LoadAssets(ContentManager content)
    {
        _hoverSoundEffect = content.Load<SoundEffect>( "ButtonHoverSound");
        _backgroundTexture = content.Load<Texture2D>(Path.Combine(Prefix, "BackgroundImage"));
        
        var startGSong = GirlModel.GetRandomSong(0, _random);
        _backgroundMusic = startGSong.Song;
        _backgroundMusicName = startGSong.Name;
        
        _movementElementPositions.Add(new Vector2(300, 150));
        _movementElementPositions.Add(new Vector2(600, 550));
        _movementElementPositions.Add(new Vector2(300, 400));
        _movementElementPositions.Add(new Vector2(300, 400));
        _movementElementPositions.Add(new Vector2(250, 400));
        
        for (int i = 0; i < Data.Girls.Count; i++)
            _movementElementPositions.Add(new Vector2(250, 400));
        
        _currentCardTexture = Data.Girls[CurrentCardIndex].Card;
        _movementElementTexture = Data.Girls[CurrentCardIndex].Me;
        _QRTexture = Data.Girls[CurrentCardIndex].Qr;
    }

    private void PlayBackgroundMusic()
    {
        MediaPlayer.Play(_backgroundMusic);
        MediaPlayer.IsRepeating = true;
    }

    private void UpdateButtons(GameTime gameTime)
    {
        _leftButton.Update(gameTime);
        _rightButton.Update(gameTime);
        _middleButton.Update(gameTime);
    }

    private void HandleInput()
    {
        if (Data.MouseState.LeftButton == ButtonState.Pressed && 
            Data.OldMouseState.LeftButton == ButtonState.Released)
        {
            if (_leftButton.Rectangle.Contains(Data.MouseState.Position))
            {
                ChangeCard(-1);
            }
            else if (_rightButton.Rectangle.Contains(Data.MouseState.Position))
            {
                ChangeCard(1);
            } 
            else if (_middleButton.Rectangle.Contains(Data.MouseState.Position))
            {
                Data.SelectedGirlId = CurrentCardIndex;
                Data.CurrentState = Core.Scenes.Girl;
            }
        }
    }

    private void HandleKeyboard()
    {
        KeyboardState keyboardState = Keyboard.GetState();
        if (!keyboardState.IsKeyDown(Keys.Escape)) return;
        
        Data.CurrentState = Core.Scenes.Start;
        MediaPlayer.Stop();
    }
    
    private void ChangeCard(int offset)
    {
        CurrentCardIndex = (CurrentCardIndex + offset + Data.Girls.Count) % Data.Girls.Count;
        var startGSong = GirlModel.GetRandomSong(CurrentCardIndex, _random);

        MediaPlayer.Stop();
        _backgroundMusic = startGSong.Song;
        _backgroundMusicName = startGSong.Name;
        Data.CurrentSongModel = startGSong;
        MediaPlayer.Play(_backgroundMusic);

        _currentCardTexture = Data.Girls[CurrentCardIndex].Card;
        _movementElementTexture = Data.Girls[CurrentCardIndex].Me;
        _QRTexture = Data.Girls[CurrentCardIndex].Qr;
    }
    private void DrawInfo(SpriteBatch spriteBatch)
    {
        spriteBatch.Draw(_backgroundMusicName,
            new Rectangle(45, 
                Data.ScreenH - _backgroundMusicName.Height,
                _backgroundMusicName.Width, _backgroundMusicName.Height),
            Color.White); 
        
        spriteBatch.Draw(_QRTexture,
            new Rectangle(Data.ScreenW - _QRTexture.Width / 3,
                Data.ScreenH - _QRTexture.Height / 3, _QRTexture.Width / 4, _QRTexture.Height / 4),
            Color.White);

    }
    private void DrawCards(SpriteBatch spriteBatch)
    {
        var scale = 0.75f;
        var blurAmount = 0.01f; 

        var centerX = Data.ScreenW / 2;
        var centerY = Data.ScreenH / 2;
        
        var currentCardPosition = new Vector2(
            centerX - _currentCardTexture.Width * scale / 2,
            centerY - _currentCardTexture.Height * scale / 2);

        var nextIndexL = (CurrentCardIndex + 1) % Data.Girls.Count;
        var nextIndexR = (CurrentCardIndex - 1 + Data.Girls.Count) % Data.Girls.Count;

        var nextCardLeftPosition = new Vector2(
            centerX + _currentCardTexture.Width * scale / 2 + 20,
            centerY - Data.Girls[nextIndexL].Card.Height * scale / 2 - Data.Girls[nextIndexL].Card.Height / 4
            );
        
        var previousCardRightPosition = new Vector2(
            centerX - _currentCardTexture.Width * scale / 2 - Data.Girls[nextIndexR].Card.Width * scale - 20,
            centerY -  Data.Girls[nextIndexR].Card.Height * scale / 2 -  Data.Girls[nextIndexR].Card.Height / 4
            );
        
        spriteBatch.Draw(Data.Girls[nextIndexL].Card, nextCardLeftPosition, 
            null, Color.White, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
        
        spriteBatch.Draw(Data.Girls[nextIndexR].Card, previousCardRightPosition,
            null, Color.White, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
        
        spriteBatch.Draw(_currentCardTexture, currentCardPosition, 
            null, Color.White, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
    }

  
}
