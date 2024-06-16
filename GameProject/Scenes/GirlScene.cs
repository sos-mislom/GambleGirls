using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using GameProject.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Newtonsoft.Json;

namespace GameProject.Scenes;

internal class GirlScene : Components
{
    private static readonly string Prefix = "GirlScene";

    private SoundEffect _hoverSoundEffect;
    private Texture2D _backgroundTexture;
    private Texture2D _rhythmGameTexture;
    private Texture2D _backButtonTexture;
    private Texture2D _casinoTexture;
    private Texture2D _puzzleBackgroundTexture;
    private Texture2D _puzzleImage;
    private Texture2D _rulesTexture;

    private Button[] _buttons = new Button[3];
    
    private List<PuzzlePieceModel> _puzzlePieces = new();
    private PuzzlePieceModel _draggingPieceModel;
    private Vector2 _dragOffset;
    private int _pieceWidth;
    private int _pieceHeight;
    
    private const int Rows = 3;
    private const int Columns = 3;
    private int _previousGirlId = -1;
    private bool _puzzleLoaded = false;
    private int _currentScore = 0;
    private float _completionPercentage = 0;
    private Texture2D _progresTexture;
    private Texture2D _scoreTexture;
    private static int _girlId = 0;
    private Texture2D _concerttexture;
    private Texture2D _puzzleInfoTexture;
    private SoundEffect _getPuzzleSound;
    private SoundEffect _setPuzzleSound;
    
    private SpriteFont _regularFont;
    private SpriteFont _smallFont;
    public void SetGirlId(int girlId) => _girlId = girlId;

    internal override void LoadContent(ContentManager content)
    {
        LoadAssets(content);
        LoadButtons(content);
        
        LoadPuzzle();
        LoadProgress();
    }
    
    
    internal override void Update(GameTime gameTime)
    {
        UpdateButtons(gameTime);
        HandleButtonClicks();
        UpdatePuzzle();
        ReloadScore();
        PrintCompletionPercentage();
    }

    internal override void Draw(SpriteBatch spriteBatch)
    {
        spriteBatch.Draw(_backgroundTexture,
            new Rectangle(0, 0, Data.ScreenW, Data.ScreenH),
            Color.White);

        spriteBatch.Draw(_puzzleBackgroundTexture,
            new Rectangle((Data.ScreenW - _puzzleBackgroundTexture.Width) / 2,
                (Data.ScreenH - _puzzleBackgroundTexture.Height) / 2,
                _puzzleBackgroundTexture.Width, _puzzleBackgroundTexture.Height),
            Color.White);

        DrawButtons(spriteBatch);
        DrawPuzzle(spriteBatch);
        DrawPercentage(spriteBatch);
        DrawScore(spriteBatch);
        DrawPuzzleInfo(spriteBatch);
    }

    private void LoadAssets(ContentManager content)
    {
        _hoverSoundEffect = content.Load<SoundEffect>("ButtonHoverSound");
        _regularFont = content.Load<SpriteFont>("RegularFont");
        _smallFont = content.Load<SpriteFont>("SmallFont");

        _rulesTexture = content.Load<Texture2D>(Path.Combine(Prefix, "Rules"));
        _progresTexture = content.Load<Texture2D>(Path.Combine(Prefix, "Progress"));
        _scoreTexture = content.Load<Texture2D>(Path.Combine(Prefix, "Score"));
        
        _concerttexture = content.Load<Texture2D>(Path.Combine(Prefix, "Concerts"));
        _puzzleInfoTexture = content.Load<Texture2D>(Path.Combine(Prefix, "NeedScore"));
        
        _getPuzzleSound = content.Load<SoundEffect>(Path.Combine(Prefix, "GetPuzzleSE"));
        _setPuzzleSound = content.Load<SoundEffect>(Path.Combine(Prefix, "SetPuzzleSE"));
        
        _backgroundTexture = Data.Girls[_girlId].BackgroundImage;
        _puzzleBackgroundTexture = Data.Girls[_girlId].PuzzleBackground;
        
        _casinoTexture = Data.Girls[_girlId].Casino;
        _backButtonTexture = Data.Girls[_girlId].BackButton;
        _rhythmGameTexture = Data.Girls[_girlId].RhythmGame;
        _puzzleImage = Data.Girls[_girlId].Puzzle;
    }

    private void LoadButtons(ContentManager content)
    {
        _buttons[0] = new Button(_backButtonTexture,
            new Rectangle(0, 0,
                _backButtonTexture.Width, _backButtonTexture.Height),
            _hoverSoundEffect);

        _buttons[1] = new Button(_casinoTexture,
            new Rectangle(
                45,
                (Data.ScreenH - _casinoTexture.Height/6) / 3,
                _casinoTexture.Width/5, _casinoTexture.Height/5),
            _hoverSoundEffect);

        _buttons[2] = new Button(_rhythmGameTexture,
            new Rectangle(60, Data.ScreenH - _rhythmGameTexture.Height/2 - 45 * 5,
                _rhythmGameTexture.Width/2, _rhythmGameTexture.Height/2),
            _hoverSoundEffect);
    }

    private void DrawButtons(SpriteBatch spriteBatch)
    {
        foreach (var button in _buttons)
            button.Draw(spriteBatch);
    }

    private void UpdateButtons(GameTime gameTime)
    {
        foreach (var button in _buttons)
            button.Update(gameTime);
    }

    private void HandleButtonClicks()
    {
        if (Data.MouseState.LeftButton == ButtonState.Pressed &&
            Data.OldMouseState.LeftButton == ButtonState.Released)
        {
            if (_buttons[0].Rectangle.Contains(Data.MouseState.Position))
            {
                Data.CurrentState = Core.Scenes.Main;
            }
            else if (_buttons[1].Rectangle.Contains(Data.MouseState.Position))
            {
                GivePuzzlePiece();
            }
            else if (_buttons[2].Rectangle.Contains(Data.MouseState.Position))
            {
                Data.CurrentState = Core.Scenes.SongChoose; 
            }
        }
    }
    
    private void GivePuzzlePiece()
    {
        var _currentScore = 0;
        
        if (File.Exists(Data.ScorePath))
            _currentScore = int.Parse(File.ReadAllText(Data.ScorePath));

        if (_currentScore >= 100)
        {
            _currentScore -= 100;
            
            var availablePieces = _puzzlePieces
                .Where(piece => !piece.IsLocked && !piece.IsVisible)
                .ToList();

            if (availablePieces.Count == 0)
                return;
        
            var random = new Random();
            var randomPiece = availablePieces[random.Next(availablePieces.Count)];
            randomPiece.IsVisible = true;
            SaveProgress();
            
            _getPuzzleSound.Play();
            File.WriteAllText(Data.ScorePath, (_currentScore).ToString());
        }
    }
    
    private void LoadPuzzle()
    {
        _pieceWidth = _puzzleImage.Width / Columns;
        _pieceHeight = _puzzleImage.Height / Rows;
        
        for (int y = 0; y < Rows; y++)
        {
            for (int x = 0; x < Columns; x++)
            {
                var pieceTexture = new Texture2D(GameEngine._graphics.GraphicsDevice, _pieceWidth, _pieceHeight);
                var pieceData = new Color[_pieceWidth * _pieceHeight];
                _puzzleImage.GetData(0, new Rectangle(x * _pieceWidth, y * _pieceHeight, _pieceWidth, _pieceHeight),
                    pieceData, 0, pieceData.Length);
                pieceTexture.SetData(pieceData);

                var position = new Vector2(Data.ScreenW - _pieceWidth - 10, y * (_pieceHeight + 10));
                var targetPosition = new Vector2(x * _pieceWidth, y * _pieceHeight);
                _puzzlePieces.Add(new PuzzlePieceModel(pieceTexture, position, targetPosition, false, false));
            }
        }
    }
    private void UpdatePuzzle()
    {
        var mouseState = Data.MouseState;
        var oldMouseState = Data.OldMouseState;

        if (mouseState.LeftButton == ButtonState.Pressed && oldMouseState.LeftButton == ButtonState.Released)
        {
            foreach (var piece in _puzzlePieces)
            {
                if (!piece.IsLocked && piece.Bounds.Contains(mouseState.Position))
                {
                    _draggingPieceModel = piece;
                    _dragOffset = mouseState.Position.ToVector2() - piece.Position;
                    break;
                }
            }
        }

        if (mouseState.LeftButton == ButtonState.Released && _draggingPieceModel != null)
        {
            var targetRect = new Rectangle((Data.ScreenW - _puzzleImage.Width) / 2,
                (Data.ScreenH - _puzzleImage.Height) / 2, _puzzleImage.Width, _puzzleImage.Height);
            var pieceTargetRect = new Rectangle((int)_draggingPieceModel.TargetPosition.X + targetRect.X,
                (int)_draggingPieceModel.TargetPosition.Y + targetRect.Y, _pieceWidth, _pieceHeight);

            if (pieceTargetRect.Contains(mouseState.Position))
            {
                _draggingPieceModel.Position = new Vector2(pieceTargetRect.X, pieceTargetRect.Y);
                _draggingPieceModel.IsLocked = true;
                
                _setPuzzleSound.Play();
                SaveProgress();
                PrintCompletionPercentage();
            }

            _draggingPieceModel = null;
        }

        if (_draggingPieceModel != null && !(_draggingPieceModel?.IsLocked ?? true))
        {
            _draggingPieceModel.Position = mouseState.Position.ToVector2() - _dragOffset;
        }
    }

    private void DrawPuzzle(SpriteBatch spriteBatch)
    {
        var targetRect = new Rectangle((Data.ScreenW - _puzzleImage.Width) / 2,
            (Data.ScreenH - _puzzleImage.Height) / 2, _puzzleImage.Width, _puzzleImage.Height);

        foreach (var piece in _puzzlePieces)
        {
            if (!piece.IsLocked && piece.IsVisible)
                spriteBatch.Draw(piece.Texture, piece.Position, Color.White);
            
            if (piece.IsLocked)
            {
                var position = new Vector2(piece.TargetPosition.X + targetRect.X,
                    piece.TargetPosition.Y + targetRect.Y);
                spriteBatch.Draw(piece.Texture, position, Color.White);
            }
        }
    }

    private void ReloadScore()
    {
        if (File.Exists(Data.ScorePath))
            _currentScore = int.Parse(File.ReadAllText(Data.ScorePath));
    }

    private void SaveProgress()
    {
        var puzzleProgress = new PuzzleProgress
        {
            Pieces = new List<(float X, float Y, bool IsLocked, bool IsVisible)>()
        };

        foreach (var piece in _puzzlePieces)
            puzzleProgress.Pieces.Add((piece.Position.X, piece.Position.Y, piece.IsLocked, piece.IsVisible));

        var json = JsonConvert.SerializeObject(puzzleProgress);

        File.WriteAllText(GetProgressFilePath(Data.SelectedGirlId), json);
    }

    private void DrawScore(SpriteBatch spriteBatch)
    {
        spriteBatch.Draw(_scoreTexture,
            new Rectangle(Data.ScreenW - 145 - _scoreTexture.Width, 10, 
                _scoreTexture.Width, _scoreTexture.Height),
            Color.White);
        
        spriteBatch.DrawString(_regularFont, $"{_currentScore}", 
            new Vector2(Data.ScreenW - 145, 45), Color.White);
    }
    private void LoadProgress()
    {
        var filePath = GetProgressFilePath(_girlId);
        
        if (!File.Exists(filePath))
            return;

        var json = File.ReadAllText(filePath);

        var puzzleProgress = JsonConvert.DeserializeObject<PuzzleProgress>(json);

        for (var i = 0; i < _puzzlePieces.Count; i++)
        {
            _puzzlePieces[i].Position = new Vector2(puzzleProgress.Pieces[i].X, puzzleProgress.Pieces[i].Y);
            _puzzlePieces[i].IsLocked = puzzleProgress.Pieces[i].IsLocked;
            _puzzlePieces[i].IsVisible = puzzleProgress.Pieces[i].IsVisible;
        }
    }

    private void DrawPuzzleInfo(SpriteBatch spriteBatch)
    {
        spriteBatch.Draw(_puzzleInfoTexture,
            new Rectangle(0, Data.ScreenH/2-_puzzleInfoTexture.Height-145, 
                _puzzleInfoTexture.Width, _puzzleInfoTexture.Height),
            Color.White);
        
        spriteBatch.DrawString(_regularFont, $"{Math.Max(100-_currentScore, 0)}", 
            new Vector2(115, Data.ScreenH / 2-_puzzleInfoTexture.Height-40), Color.Black);
    }
    private void DrawPercentage(SpriteBatch spriteBatch)
     {
         spriteBatch.Draw(_progresTexture,
             new Rectangle(10, Data.ScreenH-45-_progresTexture.Height, 
                 _progresTexture.Width, _progresTexture.Height),
             Color.White);
         
         spriteBatch.DrawString(_regularFont, $"{_completionPercentage:f2}%", 
             new Vector2(100, Data.ScreenH-75), Color.White);
     }
    private void PrintCompletionPercentage()
    {
        var lockedPieces = 0;
        foreach (var piece in _puzzlePieces)
        {
            if (piece.IsLocked)
                lockedPieces++;

            _completionPercentage = (float)lockedPieces / _puzzlePieces.Count * 100;
            Console.WriteLine($"Completion Percentage: {_completionPercentage}%");
        }
    }
    private string GetProgressFilePath(int girlId) => Path.Combine("Content", "Girls", $"progress_{girlId}.json");
    
}


