using Godot;
using System;

public static class KoordinatUtils
{
    private static int screenWidth;
    private static int screenHeight;
    private static Vector2 center;

    // Panggil ini sekali di awal permainan
    public static void Initialize(int width, int height)
    {
        screenWidth = width;
        screenHeight = height;
        center = new Vector2(width / 2, height / 2);
    }

    // Mengubah dari koordinat Kartesius (0,0 di tengah) ke koordinat Godot (0,0 di kiri atas)
    public static Vector2 CartesianToWorld(Vector2 cartesianCoord)
    {
        float worldX = cartesianCoord.X + center.X;
        // Kita balik sumbu Y karena di Godot Y positif ke bawah, di Kartesius Y positif ke atas
        float worldY = -cartesianCoord.Y + center.Y;
        return new Vector2(worldX, worldY);
    }

    // Mengubah dari koordinat Godot ke koordinat Kartesius
    public static Vector2 WorldToCartesian(Vector2 worldCoord)
    {
        float cartesianX = worldCoord.X - center.X;
        float cartesianY = -(worldCoord.Y - center.Y);
        return new Vector2(cartesianX, cartesianY);
    }
}
