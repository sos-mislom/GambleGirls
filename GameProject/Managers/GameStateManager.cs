using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.IO;
using System.Linq;
using GameProject.Core;
using GameProject.Scenes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace GameProject.Managers;

internal class GameStateManager : Components
{
    private readonly StartScene _startScene = new();
    private readonly MainScene _mainScene = new();
    private readonly SettingsScene _settingsScene = new();
    
    private GirlScene _currentGirlScene = new();
    private SongsScene _currentSongsScene = new();
    private RhythmGameScene _currentConcertScene = new();
    
    private List<GirlScene> _girlScenesList = new();
    private List<SongsScene> _songScenesList = new();
    private List<RhythmGameScene> _concertScenesList = new();
    
    private ContentManager _content; 
    internal override void LoadContent(ContentManager content)
    {
        var index = 0;
        
        while (Path.Exists(Path.Combine("Content", "Girls", index.ToString())))
        {
            var girl = new GirlModel(content, Path.Combine("Girls", index.ToString()));
            Data.Girls.Add(girl);
            index += 1;
        }
        
        for (var i = 0; i < Data.Girls.Count; i++)
        {
            var girlMenu = new GirlScene();
            girlMenu.SetGirlId(i);
            girlMenu.LoadContent(content);
            _girlScenesList.Add(girlMenu);
            
            var songsScene = new SongsScene();
            songsScene.SetGirlId(i);
            songsScene.LoadContent(content);
            _songScenesList.Add(songsScene);
            
            var concertScene = new RhythmGameScene();
            concertScene.SetGirlId(i);
            concertScene.LoadContent(content);
            _concertScenesList.Add(concertScene);
        }
       
        _startScene.LoadContent(content);
        _mainScene.LoadContent(content);
        _settingsScene.LoadContent(content);
        
        _currentGirlScene = _girlScenesList[0];
        _currentSongsScene = _songScenesList[0];
        _currentConcertScene = _concertScenesList[0];
        
        Data.OnSelectedGirlIdChanged += HandleSelectedGirlIdChanged;
    }

    private void HandleSelectedGirlIdChanged(int newGirlId)
    {
        _currentGirlScene = _girlScenesList[newGirlId];
        _currentSongsScene = _songScenesList[newGirlId];
        _currentConcertScene = _concertScenesList[newGirlId];
    }

    internal override void Update(GameTime gameTime)
    {
        switch (Data.CurrentState)
        {
            case Core.Scenes.Start:
                _startScene.Update(gameTime);
                break;
            case Core.Scenes.Main:
                _mainScene.Update(gameTime);
                break;
            case Core.Scenes.Girl:
                _currentGirlScene.Update(gameTime);
                break;
            
            case Core.Scenes.SongChoose:
                _currentSongsScene.Update(gameTime);
                break;
            case Core.Scenes.RhythmGame:
                _currentConcertScene.Update(gameTime);
                break;
            case Core.Scenes.Settings:
                _settingsScene.Update(gameTime);
                break;
        }
    }

    internal override void Draw(SpriteBatch spriteBatch)
    {
        switch (Data.CurrentState)
        {
            case Core.Scenes.Start: 
                _startScene.Draw(spriteBatch);
                break;
            case Core.Scenes.Main:
                _mainScene.Draw(spriteBatch);
                break;
            case Core.Scenes.Girl:
                _currentGirlScene.Draw(spriteBatch);
                break;
            case Core.Scenes.SongChoose:
                _currentSongsScene.Draw(spriteBatch);
                break;
            case Core.Scenes.RhythmGame:
                _currentConcertScene.Draw(spriteBatch);
                break;
            case Core.Scenes.Settings:
                _settingsScene.Draw(spriteBatch);
                break;
        }
    }
}