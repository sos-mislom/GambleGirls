using System.Collections.Generic;

namespace GameProject.Core;

public class PuzzleProgress
{
    public List<(float X, float Y, bool IsLocked, bool IsVisible)> Pieces { get; set; }
}