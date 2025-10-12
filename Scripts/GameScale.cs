using Godot;
using System;

public partial class GameScale : Node
{
    
    public static float PixelsPerCm { get; private set; }

    public static void Initialize(float scale)
    {
        PixelsPerCm = scale;
    }

}
