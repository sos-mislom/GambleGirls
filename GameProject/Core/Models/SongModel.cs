using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;

namespace GameProject.Core;

public class SongModel
{
    public Texture2D Name { get;  set; }
    public Texture2D Preview { get; set; }
    public Song Song { get; set; }
    public SongDataModel SongDataModel { get; set; }
 
    public SongModel(Texture2D name, Texture2D preview, Song song, SongDataModel songDataModel)
    {
        Name = name;
        Song = song;
        SongDataModel = songDataModel;
        Preview = preview;
    }
}
