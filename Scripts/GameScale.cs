using Godot;
using System;

public static class GameScale
{

    public static float PixelsPerCm { get; private set; }

    public static void Initialize(float scale)
    {
        PixelsPerCm = scale;
    }

}
