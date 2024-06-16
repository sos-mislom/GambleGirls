using System;
using GameProject.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

internal class Note : Components
{
    public float _initialSize; 
    public float _currentSize;
    private Texture2D _ringTexture;
    private static TimeSpan _noteDisappearTime = TimeSpan.FromSeconds(3); 

    public Rectangle Rectangle { get; private set; }
    public bool IsActive { get; set; }
    public int Score { get; private set; }
    public TimeSpan SpawnTime { get; private set; }

    private static int _prevX = -1;
    private static int _prevY = -1;
    private static int _minDist = 50; 
    private static int _maxDist = 100; 
    private static int _width = 800; 
    private static int _height = 600; 
    private static Random _random = new Random();

    public Note(GameTime gameTime)
    {
        IsActive = true;
        var position = GenerateSnakeLikePosition();

        Rectangle = new Rectangle(position.X, position.Y, 100, 100);
        
        Score = 100; 
        SpawnTime = gameTime.TotalGameTime;
                    
        _initialSize = (float)(Rectangle.Width * 1.5); 
        _currentSize = _initialSize;
    }

    private Point GenerateSnakeLikePosition()
    {
        int x, y;
        int offsetW = _width - 100; 
        int offsetH = _height - 100; 

        if (_prevX == -1 && _prevY == -1)
        {
            x = _random.Next(0, offsetW);
            y = _random.Next(0, offsetH);
        }
        else
        {
            int xMult, yMult;

            if (_prevX < _width / 4) xMult = 1;
            else if (_prevX > 3 * (_width / 4)) xMult = -1;
            else xMult = _random.Next(0, 2) == 0 ? -1 : 1;

            if (_prevY < _height / 4) yMult = 1;
            else if (_prevY > 3 * (_height / 4)) yMult = -1;
            else yMult = _random.Next(0, 2) == 0 ? -1 : 1;

            int dx = _random.Next(_minDist, _maxDist) * xMult;
            int dy = _random.Next(_minDist, _maxDist) * yMult;
            x = _prevX + dx;
            y = _prevY + dy;

            x = Math.Max(0, Math.Min(x, offsetW));
            y = Math.Max(0, Math.Min(y, offsetH));
        }

        _prevX = x;
        _prevY = y;

        return new Point(x, y);
    }
    internal override void Update(GameTime gameTime)
    {
        var elapsedSeconds = (float)(gameTime.TotalGameTime - SpawnTime).TotalSeconds + 1.2;

        _currentSize = MathHelper.Lerp(_initialSize, Rectangle.Width, 
            (float)(elapsedSeconds / (float) _noteDisappearTime.TotalSeconds));
    }
    
    internal override void LoadContent(ContentManager content)
    {
        throw new NotImplementedException();
    }
    
    internal override void Draw(SpriteBatch spriteBatch)
    {
        throw new NotImplementedException();
    }
}
