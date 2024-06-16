using System.Collections.Generic;

namespace GameProject.Core;
using MonoGame.Extended.Serialization;

public class SongDataModel
{
    public float tempo { get; set; }
    public List<float> beats { get; set; }
}