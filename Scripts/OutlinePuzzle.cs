using Godot;
using System;
using System.Collections.Generic;

public partial class OutlinePuzzle : Node2D
{

    private BentukDasar _bentukDasar = new BentukDasar();
    private Transformasi _transformasi = new Transformasi();
    private Primitif _primitif = new Primitif();
    private Color _outlineColor = new Color(0.4f, 0.4f, 0.4f);

    private Play.Level _levelToDraw;
    private bool _shouldDraw = false;

    public void DrawLevel(Play.Level level)
    {
        _levelToDraw = level;
        _shouldDraw = true;
        QueueRedraw();
    }

    public override void _Draw()
    {
        if (!_shouldDraw) return;
        switch (_levelToDraw)
        {
            case Play.Level.AirHujan:
                DrawAirHujan();
                break;
        }
    }

    private void DrawAirHujan()
    {
        // DrawSatuTetesanAir(new Vector2(-340, 50));
        DrawSatuTetesanAir(new Vector2(0, 50));
        // DrawSatuTetesanAir(new Vector2(340, 50));
    }

    // private void DrawSatuTetesanAir(Vector2 origin)
    // {
    //     // Gunakan acuan baru: sisi dasar adalah 2.0 cm
    //     float s = 2.0f * GameScale.PixelsPerCm;
    //     float h = s * Mathf.Sqrt(3) / 2; // Tinggi dari segitiga sama sisi (sekitar 1.73 cm)

    //     // Dapatkan vertices dengan ukuran yang benar
    //     var hexPoints = _bentukDasar.GetHexagonVertices(Vector2.Zero, s);
    //     var rhombusPoints = _bentukDasar.GetBelahKetupatVertices(Vector2.Zero, h * 2, s);
    //     var trianglePoints = _bentukDasar.GetSegitigaSamaSisiVertices(Vector2.Zero, s);

    //     // 1. Hexagon (Kanan)
    //     DrawComponent(hexPoints, origin + new Vector2(5, -10), 0);

    //     // 2. Belah Ketupat (3 tumpuk di kiri)
    //     DrawComponent(rhombusPoints, origin + new Vector2(-33, 56), 60);
    //     DrawComponent(rhombusPoints, origin + new Vector2(-59, 12), 60);
    //     DrawComponent(rhombusPoints, origin + new Vector2(-59, -32), -60);

    //     // 3. Segitiga (kanan atas, di atas Hexagon)
    //     DrawComponent(trianglePoints, origin + new Vector2(4, 49), 0);

    //     // 4. Segitiga (kiri atas, di puncak)
    //     DrawComponent(trianglePoints, origin + new Vector2(-21, 92), 0);
    // }
    
    private void DrawSatuTetesanAir(Vector2 origin)
    {
        float s = 2.0f * GameScale.PixelsPerCm; 
        float h = s * Mathf.Sqrt(3) / 2;

        var hexPoints = _bentukDasar.GetHexagonVertices(Vector2.Zero, s);
        var rhombusPoints = _bentukDasar.GetBelahKetupatVertices(Vector2.Zero, h * 2, s);
        var trianglePoints = _bentukDasar.GetSegitigaSamaSisiVertices(Vector2.Zero, s);

        // Kirim ShapeType yang sesuai ke DrawComponent
        DrawComponent(hexPoints, Bentuk.ShapeType.Hexagon, origin + new Vector2(5, -10), 0);
        DrawComponent(rhombusPoints, Bentuk.ShapeType.BelahKetupat, origin + new Vector2(-33, 56), 60);
        DrawComponent(rhombusPoints, Bentuk.ShapeType.BelahKetupat, origin + new Vector2(-59, 12), 60);
        DrawComponent(rhombusPoints, Bentuk.ShapeType.BelahKetupat, origin + new Vector2(-59, -32), -60);
        DrawComponent(trianglePoints, Bentuk.ShapeType.SegitigaSamaSisi, origin + new Vector2(4, 49), 0);
        DrawComponent(trianglePoints, Bentuk.ShapeType.SegitigaSamaSisi, origin + new Vector2(-21, 92), 0);
    }

    private void DrawComponent(List<Vector2> originalVertices, Bentuk.ShapeType shapeType, Vector2 cartesianPos, float rotationDegrees)
    {
        var matrix = new float[3, 3];
        Transformasi.Matrix3x3Identity(matrix);
        var dummyPivot = Vector2.Zero;

        if (rotationDegrees != 0)
        {
            _transformasi.RotationClockwise(matrix, rotationDegrees, dummyPivot);
        }
        _transformasi.Translation(matrix, cartesianPos.X, cartesianPos.Y, ref dummyPivot);

        var transformedPoints = _transformasi.GetTransformPoint(matrix, originalVertices);
        var worldPoints = new List<Vector2>();
        foreach (var p in transformedPoints)
        {
            worldPoints.Add(KoordinatUtils.CartesianToWorld(p));
        }

        // --- Bagian Menggambar Outline (Tetap Sama) ---
        var pixelPoints = _primitif.Polygon(worldPoints);
        foreach (var point in pixelPoints)
        {
            DrawRect(new Rect2(point, Vector2.One), _outlineColor);
        }

        // --- BAGIAN BARU: Membuat Slot Area2D ---
        var slot = new Slot();
        slot.Name = shapeType.ToString() + "Slot"; // Beri nama unik
        slot.TargetShape = shapeType;
        slot.TargetMatrix = matrix; // Simpan matriks "kunci jawaban"
        AddChild(slot);

        var collisionShape = new CollisionShape2D();
        var polygonShape = new ConvexPolygonShape2D();
        polygonShape.Points = worldPoints.ToArray();
        collisionShape.Shape = polygonShape;
        slot.AddChild(collisionShape);
    }

}
