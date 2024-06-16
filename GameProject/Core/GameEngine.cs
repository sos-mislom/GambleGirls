using GameProject.Managers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace GameProject.Core;

public class GameEngine : Game
{
    public static GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;
    private GameStateManager _gsm;
    private Texture2D _cursorTexture;
    public GameEngine()
    {
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = false;
    }

    protected override void Initialize()
    {
        _graphics.PreferredBackBufferWidth = Data.ScreenW;
        _graphics.PreferredBackBufferHeight = Data.ScreenH;
        _graphics.ApplyChanges();
        _gsm = new GameStateManager();
        base.Initialize();
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);
        _cursorTexture = Content.Load<Texture2D>( "CursorTexture");
        _gsm.LoadContent(Content);
    }

    protected override void Update(GameTime gameTime)
    {
        Data.OldMouseState = Data.MouseState;
        Data.MouseState = Mouse.GetState();
                
        _gsm.Update(gameTime);
        
        if (Data.Exit)
            Exit();

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.White);

        _spriteBatch.Begin();
        _gsm.Draw(_spriteBatch);
        DrawCursor(_spriteBatch);
        _spriteBatch.End();

        base.Draw(gameTime);
    }
    
    private void DrawCursor(SpriteBatch spriteBatch)
    {
        var cursorPosition = new Vector2(Data.MouseState.X, Data.MouseState.Y);
        spriteBatch.Draw(_cursorTexture, cursorPosition, null,
            Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
    }
}