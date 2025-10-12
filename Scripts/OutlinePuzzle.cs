using Godot;
using System;
using System.Collections.Generic;

public partial class OutlinePuzzle : Node2D
{
    
    // private BentukDasar _bentukDasar = new BentukDasar();
    // private Transformasi _transformasi = new Transformasi();
    // private Primitif _primitif = new Primitif();
    // private Color _outlineColor = new Color(0.4f, 0.4f, 0.4f);

    // public override void _Draw()
    // {
    //     // Gambar Sumbu Kartesius dengan warna yang lebih terang
    //     var viewportSize = GetViewportRect().Size;
    //     var axesPoints = _primitif.CartesianAxes((int)viewportSize.X, (int)viewportSize.Y);
    //     foreach (var p in axesPoints)
    //     {
    //         DrawRect(new Rect2(p, Vector2.One), Colors.White, true);
    //     }

    //     // Gambar 3 pola tetesan air di posisi Kartesius
    //     DrawRaindrop(new Vector2(-340, 0));
    //     DrawRaindrop(new Vector2(0, 0));
    //     DrawRaindrop(new Vector2(340, 0));
    // }

    // private void DrawRaindrop(Vector2 cartesianOrigin)
    // {
    //     float side = 50; // Ukuran sisi standar

    //     // 1. Dapatkan titik-titik dasar untuk setiap bentuk, berpusat di (0,0)
    //     var hexPoints = _bentukDasar.Hexagon(Vector2.Zero, side);
    //     var rhombusPoints = _bentukDasar.BelahKetupat(Vector2.Zero, side);
    //     var trianglePoints = _bentukDasar.SegitigaSamaSisi(Vector2.Zero, side);

    //     // --- Susun setiap komponen menggunakan Transformasi.cs ---
    //     // Variabel dummy untuk ref, karena kita bekerja dalam Kartesius
    //     Vector2 pivot = Vector2.Zero;

    //     // A. Gambar Hexagon di tengah
    //     var hexMatrix = new float[3, 3];
    //     Transformasi.Matrix3x3Identity(hexMatrix);
    //     _transformasi.Translation(hexMatrix, cartesianOrigin.X, cartesianOrigin.Y, ref pivot);
    //     DrawComponent(hexPoints, hexMatrix);

    //     // B. Gambar Belah Ketupat Kiri
    //     var rhom1Matrix = new float[3, 3];
    //     Transformasi.Matrix3x3Identity(rhom1Matrix);
    //     _transformasi.Translation(rhom1Matrix, cartesianOrigin.X - (side * 0.75f), cartesianOrigin.Y + (side * 0.433f), ref pivot);
    //     _transformasi.RotationClockwise(rhom1Matrix, 30, pivot); // Rotasi 30 derajat
    //     DrawComponent(rhombusPoints, rhom1Matrix);
        
    //     // C. Gambar Belah Ketupat Kanan
    //     var rhom2Matrix = new float[3, 3];
    //     Transformasi.Matrix3x3Identity(rhom2Matrix);
    //     _transformasi.Translation(rhom2Matrix, cartesianOrigin.X + (side * 0.75f), cartesianOrigin.Y + (side * 0.433f), ref pivot);
    //     _transformasi.RotationClockwise(rhom2Matrix, -30, pivot); // Rotasi -30 derajat
    //     DrawComponent(rhombusPoints, rhom2Matrix);

    //     // D. Gambar Segitiga
    //     var triMatrix = new float[3, 3];
    //     Transformasi.Matrix3x3Identity(triMatrix);
    //     _transformasi.Translation(triMatrix, cartesianOrigin.X, cartesianOrigin.Y + (side * 0.866f), ref pivot);
    //     DrawComponent(trianglePoints, triMatrix);
    // }

    // private void DrawComponent(List<Vector2> cartesianPoints, float[,] matrix)
    // {
    //     var transformedCartesianPoints = _transformasi.GetTransformPoint(matrix, cartesianPoints);

    //     var worldPoints = new List<Vector2>();
    //     foreach (var p in transformedCartesianPoints)
    //     {
    //         worldPoints.Add(KoordinatUtils.CartesianToWorld(p));
    //     }
        
    //     foreach (var point in worldPoints)
    //     {
    //         DrawRect(new Rect2(point, Vector2.One), _outlineColor);
    //     }
    // }

    private BentukDasar _bentukDasar = new BentukDasar();
    private Transformasi _transformasi = new Transformasi();
    private Primitif _primitif = new Primitif();
    private Color _outlineColor = new Color(0.4f, 0.4f, 0.4f);

    private Play.Level _levelToDraw;
    private bool _shouldDraw = false;

    // Fungsi ini akan dipanggil dari Play.cs
    public void DrawLevel(Play.Level level)
    {
        _levelToDraw = level;
        _shouldDraw = true;
        QueueRedraw(); // Minta untuk menggambar ulang
    }

    public override void _Draw()
    {
        if (!_shouldDraw) return;

        // Gambar Sumbu Kartesius
        var viewportSize = GetViewportRect().Size;
        var axesPoints = _primitif.CartesianAxes((int)viewportSize.X, (int)viewportSize.Y);
        foreach (var p in axesPoints)
        {
            DrawRect(new Rect2(p, Vector2.One), Colors.White, true);
        }

        // Pilih fungsi gambar berdasarkan level
        switch (_levelToDraw)
        {
            case Play.Level.AirHujan:
                DrawAirHujan();
                break;
            case Play.Level.Payung:
                DrawPayung();
                break;
            case Play.Level.PenyiramTanaman:
                DrawPenyiramTanaman();
                break;
        }
    }

    // --- FUNGSI-FUNGSI GAMBAR PER LEVEL ---

    private void DrawAirHujan()
    {
        DrawTetesanAir(new Vector2(-340, 50));
        DrawTetesanAir(new Vector2(0, 50));
        DrawTetesanAir(new Vector2(340, 50));
    }

    private void DrawPayung()
    {
        // Implementasi gambar outline payung
        // Anda bisa menambahkan logikanya di sini
        GD.Print("Menggambar Payung (belum diimplementasi)");
    }

    private void DrawPenyiramTanaman()
    {
        // Implementasi gambar outline penyiram tanaman
        // Anda bisa menambahkan logikanya di sini
        GD.Print("Menggambar Penyiram Tanaman (belum diimplementasi)");
    }

    // --- FUNGSI HELPER ---

    private void DrawTetesanAir(Vector2 origin)
    {
        // Gunakan ukuran dari spesifikasi cm
        float triangleSide = 2.4f * GameScale.PixelsPerCm;
        float hexSide = 2.5f * GameScale.PixelsPerCm;
        float h = triangleSide * Mathf.Sqrt(3) / 2; // Tinggi dari segitiga sama sisi

        // Dapatkan vertices dasar untuk setiap bentuk
        var hexPoints = _bentukDasar.GetHexagonVertices(Vector2.Zero, hexSide);
        // PERBAIKAN: Belah ketupat harusnya menggunakan triangleSide
        var rhombusPoints = _bentukDasar.GetBelahKetupatVertices(Vector2.Zero, triangleSide); 
        var trianglePoints = _bentukDasar.GetSegitigaSamaSisiVertices(Vector2.Zero, triangleSide);
        
        // --- Susun Pola Sesuai Gambar Referensi ---

        // 1. Hexagon Kuning (paling bawah)
        DrawComponent(hexPoints, origin + new Vector2(0, -h), 0);
        
        // 2. Belah Ketupat Biru (3 buah, di sebelah kiri)
        DrawComponent(rhombusPoints, origin + new Vector2(-triangleSide / 2, 0), -30);
        DrawComponent(rhombusPoints, origin + new Vector2(-triangleSide / 2, h), -30);
        DrawComponent(rhombusPoints, origin + new Vector2(-triangleSide / 2, h * 2), -30);

        // 3. Segitiga Hijau (2 buah, di sebelah kanan)
        DrawComponent(trianglePoints, origin + new Vector2(triangleSide / 2, 0), 0);
        DrawComponent(trianglePoints, origin + new Vector2(triangleSide / 2, h), 0);
    }

    private void DrawComponent(List<Vector2> originalVertices, Vector2 cartesianPos, float rotationDegrees)
    {
        var matrix = new float[3, 3];
        Transformasi.Matrix3x3Identity(matrix);
        var dummyPivot = Vector2.Zero;

        // Terapkan rotasi terlebih dahulu jika ada
        if (rotationDegrees != 0)
        {
            _transformasi.RotationClockwise(matrix, rotationDegrees, dummyPivot);
        }
        // Kemudian terapkan translasi
        _transformasi.Translation(matrix, cartesianPos.X, cartesianPos.Y, ref dummyPivot);

        var transformedPoints = _transformasi.GetTransformPoint(matrix, originalVertices);

        var worldPoints = new List<Vector2>();
        foreach (var p in transformedPoints)
        {
            worldPoints.Add(KoordinatUtils.CartesianToWorld(p));
        }
        
        // Gambar garis-garis poligon
        var pixelPoints = _primitif.Polygon(worldPoints);
        foreach (var point in pixelPoints)
        {
            DrawRect(new Rect2(point, Vector2.One), _outlineColor);
        }
    }

}
