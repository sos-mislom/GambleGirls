using System;
using System.Collections.Generic;
using System.IO;
using GameProject.Managers;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace GameProject.Core;

public class Data
{
    public static Scenes CurrentState { get; set; } = Scenes.Start;
    public static SongModel CurrentSongModel { get; set; }
    public static List<GirlModel> Girls { get; set; } = new();
    public static int ScreenW { get; set; } = 1100;
    public static int ScreenH { get; set; } = 900;
    public static bool Exit { get; set; } = false;
    
    public static MouseState MouseState;
    public static MouseState OldMouseState;
    
    public static String ScorePath = Path.Combine("Content", "Girls", "score.json");
    
    private static int _selectedGirlId;
    public static int SelectedGirlId
    {
        get { return _selectedGirlId; }
        set
        {
            if (_selectedGirlId != value)
            {
                _selectedGirlId = value;
                OnSelectedGirlIdChanged?.Invoke(_selectedGirlId);
            }
        }
    }
    public static event Action<int> OnSelectedGirlIdChanged;
    
}