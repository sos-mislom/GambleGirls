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
using Microsoft.Xna.Framework.Media;

namespace GameProject.Scenes;

internal class RhythmGameScene : Components
{
    private static readonly String Prefix = "RhythmGameScene";
    private SongModel _songModel;
    private int _currentBeatIndex;
    private bool _isMusicPlaying = false;
    private bool _isPaused = false;
    private Texture2D _warningTexture;
    private SpriteFont _font;
    private int _score = 0;
    private int _combo = 0;
    private int _finalScore = 0;
    private static Texture2D _missTexture;
    private static Texture2D _badTexture;
    private static Texture2D _greatTexture;
    private static Texture2D _ringTexture;
    private static Texture2D _noteTexture;
    private List<Note> _notes = new List<Note>();
    private Queue<int> _expectedNotes = new Queue<int>();
    private TimeSpan _nextNoteSpawnTime = TimeSpan.Zero;
    private static TimeSpan _noteDisappearTime = TimeSpan.FromSeconds(1);
    private TimeSpan _totalTrackDuration;
    private float _beatsPerMinute;
    private static Random _random = new Random();
    private List<Grade> _gradeAnimations = new List<Grade>();
    private List<float> _beats;

    private bool flag = true;
    private TimeSpan _lastNoteSpawnTime = TimeSpan.Zero;
    private Texture2D _backgroundTexture;
    private Texture2D _pauseMenuTexture;
    private Texture2D _finalScoreTexture;
    private Button _button_back;
    private Button _button_resume;
    private bool _isFinish = false;
    private Texture2D _endMenuTexture;
    private Button _button_exit;
    private Texture2D _progressTexture;

    private static int _girlId = 0;
    public void SetGirlId(int girlId) => _girlId = girlId;
    internal override void LoadContent(ContentManager content)
    {
        _backgroundTexture = Data.Girls[_girlId].ConcertBackground;
        _noteTexture = content.Load<Texture2D>(Path.Combine(Prefix, "NoteTexture"));
        _ringTexture = content.Load<Texture2D>(Path.Combine(Prefix, "RingTexture"));

        _warningTexture = content.Load<Texture2D>(Path.Combine(Prefix, "Warning"));
        
        _pauseMenuTexture = content.Load<Texture2D>(Path.Combine(Prefix, "Pause"));
        _finalScoreTexture = content.Load<Texture2D>(Path.Combine(Prefix, "End"));
        
        _progressTexture = content.Load<Texture2D>(Path.Combine(Prefix, "Progress"));
        
        _endMenuTexture = content.Load<Texture2D>(Path.Combine(Prefix,"Pause"));
        
        _missTexture = content.Load<Texture2D>(Path.Combine(Prefix,"MissTexture"));
        _badTexture = content.Load<Texture2D>(Path.Combine(Prefix,"BadTexture"));
        _greatTexture = content.Load<Texture2D>(Path.Combine(Prefix,"GreatTexture"));
        
        var buttonBackTexture = content.Load<Texture2D>(Path.Combine(Prefix, "Back"));
        var buttonResumeTexture = content.Load<Texture2D>(Path.Combine(Prefix, "Resume"));

        var buttonBackRect = new Rectangle(
            (Data.ScreenW - buttonBackTexture.Width)/2 + 300, 
            (Data.ScreenH-buttonBackTexture.Height)/2, 
            buttonBackTexture.Width, buttonBackTexture.Height);
        _button_back = new Button(buttonBackTexture, buttonBackRect,
            content.Load<SoundEffect>("ButtonHoverSound"));
            
        var exitRectangle = new Rectangle(
            (Data.ScreenW - buttonBackTexture.Width)/2, 
            (Data.ScreenH-buttonBackTexture.Height)/2, 
            buttonBackTexture.Width, buttonBackTexture.Height);
        _button_exit = new Button(buttonBackTexture, exitRectangle, 
            content.Load<SoundEffect>("ButtonHoverSound"));
        
        var buttonResumeRect = new Rectangle(
            (Data.ScreenW - buttonResumeTexture.Width)/2 - 300, 
            (Data.ScreenH-buttonResumeTexture.Height)/2, 
            buttonResumeTexture.Width, buttonResumeTexture.Height);
        _button_resume = new Button(buttonResumeTexture, buttonResumeRect, 
            content.Load<SoundEffect>("ButtonHoverSound"));


        _font = content.Load<SpriteFont>("RegularFont");

        _songModel = Data.CurrentSongModel;
    }

    private void UpdateParams()
    {
        _isFinish = false;
        _isMusicPlaying = false;
        _songModel = Data.CurrentSongModel;
        _beats = _songModel.SongDataModel.beats;
         flag = false;
        _totalTrackDuration = TimeSpan.FromSeconds(_beats.Last());
        _beatsPerMinute = _beats.Count / (float)_totalTrackDuration.TotalMinutes;
        _score = 0;
        _currentBeatIndex = 0;
        _expectedNotes = new Queue<int>();
        _finalScore = 0;
        _nextNoteSpawnTime = TimeSpan.FromSeconds(_beats[0] / _beatsPerMinute);
    }

    internal override void Update(GameTime gameTime)
    {
        if (flag)
            UpdateParams();

        if (_isFinish)
        {
            UpdateButtons(gameTime);
            HandleButtonClicks();
            return;
        }
        
        if (!_isPaused)
        {
            if (!_isMusicPlaying && Keyboard.GetState().IsKeyDown(Keys.Space))
            {
                MediaPlayer.Play(_songModel.Song);
                _isMusicPlaying = true;
            }
        
            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
            {
                _isPaused = true;
                MediaPlayer.Pause();
            }

            if (_isMusicPlaying)
            {
                GenerateNotes(gameTime);
                UpdateNotes(gameTime);
                CalculatePoints();
            
                if (_currentBeatIndex >= _beats.Count)
                {
                    SaveScore();
                    _isFinish = true;
                    
                    MediaPlayer.Stop();
                    _isMusicPlaying = false;
                }
            }
        }
        else
        {
            if (Keyboard.GetState().IsKeyUp(Keys.Escape))
            {
                _isPaused = false;
                MediaPlayer.Resume();
            }

            UpdateButtons(gameTime);
            HandleButtonClicks();
        }
        
        foreach (var gradeAnimation in _gradeAnimations)
            gradeAnimation.Update();
        
        _gradeAnimations.RemoveAll(gradeAnimation => !gradeAnimation.IsActive);
    }
    internal override void Draw(SpriteBatch spriteBatch)
    {
        DrawBackground(spriteBatch);
        DrawNotes(spriteBatch);
        DrawGrade(spriteBatch);
        DrawWarn(spriteBatch);
        DrawScore(spriteBatch);
        DrawProgress(spriteBatch);
        
        if (_isFinish)
            DrawFinalScore(spriteBatch);
        
        if (_isPaused)
            DrawPauseMenu(spriteBatch);
    }

    private void GenerateNotes(GameTime gameTime)
    {
        if (_currentBeatIndex < _beats.Count && MediaPlayer.State == MediaState.Playing)
        {
            var nextBeatTime = TimeSpan.FromSeconds(_beats[_currentBeatIndex]);

            var noteSpawnInterval = nextBeatTime - (_currentBeatIndex > 0 ? TimeSpan.FromSeconds(_beats[_currentBeatIndex - 1]) : TimeSpan.Zero);

            if (gameTime.TotalGameTime - _lastNoteSpawnTime > noteSpawnInterval)
            {
                _lastNoteSpawnTime = gameTime.TotalGameTime;

                Note newNote = null;
                var isValidPosition = false;

                while (!isValidPosition)
                {
                    newNote = new Note(gameTime);

                    var isOverlap = false;
                    foreach (var existingNote in _notes)
                    {
                        if (newNote.Rectangle.Intersects(existingNote.Rectangle))
                        {
                            isOverlap = true;
                            break;
                        }
                    }

                    if (!isOverlap)
                    {
                        isValidPosition = true;
                    }
                }

                _notes.Add(newNote);
                _expectedNotes.Enqueue(newNote.GetHashCode());

                _currentBeatIndex++;
            }
        }
    }
    
    private void HandleButtonClicks()
    {
        if (Data.MouseState.LeftButton == ButtonState.Pressed)
        {
            if (_button_back.Rectangle.Contains(Data.MouseState.Position) || 
                _button_exit.Rectangle.Contains(Data.MouseState.Position))
            {
                flag = true;
                Data.CurrentState = Core.Scenes.SongChoose;
                SaveScore();
                MediaPlayer.Stop();
            }
            else if (_button_resume.Rectangle.Contains(Data.MouseState.Position))
            {
                MediaPlayer.Resume();
                _isPaused = false;
            }
        }
    }
    private void UpdateButtons(GameTime gameTime)
    {
        _button_resume.Update(gameTime);
        _button_back.Update(gameTime);
        _button_exit.Update(gameTime);
    }
    
    private void UpdateNotes(GameTime gameTime)
    {
        foreach (var note in _notes)
        {
            note.Update(gameTime);
            var grade = _missTexture;
            if (note.IsActive && Data.MouseState.LeftButton == ButtonState.Pressed
                              && Data.OldMouseState.LeftButton == ButtonState.Released
                              && note.Rectangle.Contains(Data.MouseState.Position))
            {

                if (gameTime.TotalGameTime - note.SpawnTime >= TimeSpan.FromSeconds(0.6))
                {
                    note.IsActive = false;
                    _combo++;
                    _score++;
                }
                else
                {
                    note.IsActive = false;
                    _combo = 0;
                    _expectedNotes.Clear();
                }

                if (gameTime.TotalGameTime - note.SpawnTime >= TimeSpan.FromSeconds(0.6))
                {
                    grade = _greatTexture;
                }
                else if (gameTime.TotalGameTime - note.SpawnTime >= TimeSpan.FromSeconds(0))
                {
                    grade = _badTexture;
                }

                var gradeAnimation = new Grade(grade, new Vector2(note.Rectangle.X, note.Rectangle.Y));
                _gradeAnimations.Add(gradeAnimation);

            }
            if (gameTime.TotalGameTime - note.SpawnTime > _noteDisappearTime && note.IsActive)
            {
                _combo = 0;
                grade = _missTexture;
                var gradeAnimation = new Grade(grade, new Vector2(note.Rectangle.X, note.Rectangle.Y));
                _gradeAnimations.Add(gradeAnimation);
            }

        }

        _notes.RemoveAll(note => gameTime.TotalGameTime - note.SpawnTime > _noteDisappearTime);

        if (_notes.All(note => !note.IsActive))
            _currentBeatIndex++;
    }

    private void DrawNotes(SpriteBatch spriteBatch)
    {
        foreach (var note in _notes)
        {
            if (note.IsActive)
            {
                var borderRectangle = new Rectangle(
                    note.Rectangle.X + note.Rectangle.Width / 2 - (int)(note._currentSize / 2),
                    note.Rectangle.Y + note.Rectangle.Height / 2 - (int)(note._currentSize / 2),
                    (int)note._currentSize,
                    (int)note._currentSize);

                spriteBatch.Draw(_noteTexture, note.Rectangle, Color.White);
                spriteBatch.Draw(_ringTexture, borderRectangle, Color.White * 0.5f);
            }
        }
    }

    private void DrawWarn(SpriteBatch spriteBatch)
    {
        if (!_isMusicPlaying)
        {
            spriteBatch.Draw(_warningTexture,
                new Rectangle(
                    (Data.ScreenW - _warningTexture.Width) / 2,
                    (Data.ScreenH - _warningTexture.Height) / 2,
                    _warningTexture.Width, _warningTexture.Height),
                Color.White);
        }
    }

    private void DrawGrade(SpriteBatch spriteBatch)
    {
        foreach (var gradeAnimation in _gradeAnimations)
        {
            spriteBatch.Draw(gradeAnimation.Texture, gradeAnimation.Position, Color.White * gradeAnimation.Alpha);
        }
    }

    private void DrawScore(SpriteBatch spriteBatch)
    {
        spriteBatch.DrawString(_font, $"{_combo}x", new Vector2(10, 10), Color.White);
        //spriteBatch.DrawString(_font, $"{_finalScore} / 100", new Vector2(10, 30), Color.White);
    }

    private void DrawProgress(SpriteBatch spriteBatch)
    {
        if (_isMusicPlaying)
        {
            var progress = (float)_currentBeatIndex / _beats.Count;
            
            var progressBarRectangle = new Rectangle(10, 80,
                (int)(Data.ScreenW * progress), 20);
            spriteBatch.Draw(_progressTexture, progressBarRectangle, Color.White);
            
            var pp = new Rectangle((int)(Data.ScreenW * progress)-10, 65, 
                _ringTexture.Width/5, _ringTexture.Height/5);
            spriteBatch.Draw(_ringTexture, pp, Color.White);
        }
    }

    private void DrawPauseMenu(SpriteBatch spriteBatch)
    {
        spriteBatch.Draw(_endMenuTexture,
            new Rectangle(
                (Data.ScreenW - _endMenuTexture.Width) / 2,
                (Data.ScreenH - _endMenuTexture.Height) / 2,
                _endMenuTexture.Width, _endMenuTexture.Height),
            Color.White);
        
        spriteBatch.DrawString(_font, $"{_finalScore} / 100", 
            new Vector2(Data.ScreenW/2 - 100, Data.ScreenW/2 - 100),
            Color.White);
        
        _button_back.Draw(spriteBatch);
        _button_resume.Draw(spriteBatch);
    }

    private void DrawFinalScore(SpriteBatch spriteBatch)
    {
        spriteBatch.Draw(_pauseMenuTexture,
            new Rectangle(
                (Data.ScreenW - _pauseMenuTexture.Width) / 2,
                (Data.ScreenH - _pauseMenuTexture.Height) / 2,
                _pauseMenuTexture.Width, _pauseMenuTexture.Height),
            Color.White);
        
        spriteBatch.DrawString(_font, $"{_finalScore} / 100", 
            new Vector2(Data.ScreenW/2, Data.ScreenW/3),
            Color.White);
        
        _button_exit.Draw(spriteBatch);
        CalculatePoints();
    }
    
    private void CalculatePoints()
    {
        var percentage = (double) _score / _beats.Count;
        _finalScore = (int)Math.Ceiling(percentage * 10) * ((int)_songModel.SongDataModel.tempo / 10);
    }
    
    private void SaveScore()
    {
        var globalScore = 0;
        
        if (File.Exists(Data.ScorePath))
            globalScore = int.Parse(File.ReadAllText(Data.ScorePath));
        
        File.WriteAllText(Data.ScorePath, (_finalScore+globalScore).ToString());
    }
    
    
    private void DrawBackground(SpriteBatch spriteBatch)
    {
        spriteBatch.Draw(_backgroundTexture,
            new Rectangle(0, 0, Data.ScreenW, Data.ScreenH),
            Color.White);
    }
}
