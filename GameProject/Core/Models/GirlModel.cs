using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;

using System.Text.Json;
namespace GameProject.Core;

public class GirlModel
{
    public Texture2D Qr { get; private set; }
    public Texture2D BackgroundImage { get; private set; }
    public Texture2D Card { get; private set; }
    public Texture2D Me { get; private set; }
    public Texture2D Casino { get; private set; }
    public Texture2D BackButton { get; private set; }
    public Texture2D RhythmGame { get; private set; }
    public Texture2D PuzzleBackground { get; private set; }
    public Texture2D ConcertBackground { get; private set; }
    public Texture2D Puzzle { get; private set; }
    public List<SongModel> Songs { get; private set; }

    public GirlModel(ContentManager content, string folderPath)
    {
        Qr = content.Load<Texture2D>(Path.Combine(folderPath, "QR"));
        BackgroundImage = content.Load<Texture2D>(Path.Combine(folderPath, "BackgroundImage"));
        Card = content.Load<Texture2D>(Path.Combine(folderPath, "Card"));
        Me = content.Load<Texture2D>(Path.Combine(folderPath, "ME"));
        Casino = content.Load<Texture2D>(Path.Combine(folderPath, "Casino"));
        BackButton = content.Load<Texture2D>(Path.Combine(folderPath, "BackButton"));
        ConcertBackground = content.Load<Texture2D>(Path.Combine(folderPath, "ConcertBackground"));
        RhythmGame = content.Load<Texture2D>(Path.Combine(folderPath, "RhythmGame"));
        PuzzleBackground = content.Load<Texture2D>(Path.Combine(folderPath, "PuzzleBackground"));
        Puzzle = content.Load<Texture2D>(Path.Combine(folderPath, "Puzzle"));
        Songs = LoadSongs(content, folderPath);
    }
    
    private List<SongModel> LoadSongs(ContentManager content, string folderPath)
    {
        folderPath = Path.Combine(folderPath, "Songs");
        var songs = new List<SongModel>();
        
        for (var i = 0; i < 4; i++)
        {
            var path = Path.Combine(folderPath, i.ToString());
            
            var songName = content.Load<Texture2D>(Path.Combine(path, "SongName"));
            
            var preview = content.Load<Texture2D>(Path.Combine(path, "Preview"));
            var song = content.Load<Song>(Path.Combine(path, "Song"));
      
            var jsonFilePath = Path.Combine("Content", path, "song_analysis.json");
            var songTempo = new SongDataModel();
            
            
            if (File.Exists(jsonFilePath))
            {
                var jsonString = File.ReadAllText(jsonFilePath);
                var analysisData = JsonSerializer.Deserialize<SongDataModel>(jsonString);
                
                if (analysisData != null)
                    songTempo = analysisData;
            }
            
            songs.Add(new SongModel(songName, preview, song, songTempo));
        }

        return songs;
    }
    
    public static SongModel GetRandomSong(int girlIndex, Random random)
    {
        if (girlIndex < 0 || girlIndex >= Data.Girls.Count)
            throw new ArgumentOutOfRangeException(nameof(girlIndex), "Invalid girl index");

        var girl = Data.Girls[girlIndex];
        if (girl.Songs == null || girl.Songs.Count == 0)
            throw new InvalidOperationException("No songs available for the selected girl");
        
        var randomIndex = random.Next(girl.Songs.Count);
        return girl.Songs[randomIndex];
    }
}
