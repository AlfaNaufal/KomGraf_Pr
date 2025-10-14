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

    public int TotalSlots { get; private set; } = 0;

    [Signal]
    public delegate void OutlineDrawnEventHandler();

    public void DrawLevel(Play.Level level)
    {
        TotalSlots = 0; 
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
            case Play.Level.Payung:
                DrawPayung();
                break;
            case Play.Level.PenyiramTanaman:
                DrawPenyiramTanaman();
                break;
        }

        EmitSignal(SignalName.OutlineDrawn);
        _shouldDraw = false; // Mencegah penggambaran berulang
    }

    private void DrawAirHujan()
    {
        DrawSatuTetesanAir(new Vector2(0, 50));
    }

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
    
    private void DrawPayung()
    {
        DrawSatuPayung(new Vector2(0, 100));
    }

    private void DrawSatuPayung(Vector2 origin)
    {
        float s = 2.0f * GameScale.PixelsPerCm;
        float h = s * Mathf.Sqrt(3) / 2;

        var hexPoints = _bentukDasar.GetHexagonVertices(Vector2.Zero, s);
        var trapPoints = _bentukDasar.GetTrapesiumSamaKakiVertices(Vector2.Zero, s, s * 2, h);
        var rhombusPoints = _bentukDasar.GetBelahKetupatVertices(Vector2.Zero, h * 2, s);
        var trianglePoints = _bentukDasar.GetSegitigaSamaSisiVertices(Vector2.Zero, s);
        var jajarGenjangPoints = _bentukDasar.GetJajarGenjangVertices(Vector2.Zero, s, h, s / 2);

        // --- Susun Pola Payung (Perhitungan Akurat) ---
        // (origin adalah pusat dari Hexagon)

        // 1. Hexagon (Kuning)
        DrawComponent(hexPoints, Bentuk.ShapeType.Hexagon, origin, 90);

        // 2. Trapesium (Merah, 2 buah)
        DrawComponent(trapPoints, Bentuk.ShapeType.TrapesiumSamaKaki, origin + new Vector2(-68, -50), 90);
        DrawComponent(trapPoints, Bentuk.ShapeType.TrapesiumSamaKaki, origin + new Vector2(69, -50), -90);

        // 3. Belah Ketupat (Biru, Kiri dan Kanan)
        DrawComponent(rhombusPoints, Bentuk.ShapeType.BelahKetupat, origin + new Vector2(-24, -62), -30);
        DrawComponent(rhombusPoints, Bentuk.ShapeType.BelahKetupat, origin + new Vector2(-113, -64), -30);
        DrawComponent(rhombusPoints, Bentuk.ShapeType.BelahKetupat, origin + new Vector2(24, -62), 30);
        DrawComponent(rhombusPoints, Bentuk.ShapeType.BelahKetupat, origin + new Vector2(113, -64), 30);

        // 4. Segitiga (Hijau, Kiri dan Kanan Atas)
        DrawComponent(trianglePoints, Bentuk.ShapeType.SegitigaSamaSisi, origin + new Vector2(-60, 0), 29);
        DrawComponent(trianglePoints, Bentuk.ShapeType.SegitigaSamaSisi, origin + new Vector2(-105, -25), 30);
        DrawComponent(trianglePoints, Bentuk.ShapeType.SegitigaSamaSisi, origin + new Vector2(60, 0), -29);
        DrawComponent(trianglePoints, Bentuk.ShapeType.SegitigaSamaSisi, origin + new Vector2(105, -25), -30);

        // 5. Belah Ketupat (Biru, Gagang Payung)
        DrawComponent(rhombusPoints, Bentuk.ShapeType.BelahKetupat, origin + new Vector2(0, -145), 0);
    }

    private void DrawPenyiramTanaman()
    {
        DrawSatuPenyiramTanaman(new Vector2(0, 50));
    }

    private void DrawSatuPenyiramTanaman(Vector2 origin)
    {
        float s = 2.0f * GameScale.PixelsPerCm;
        float h = s * Mathf.Sqrt(3) / 2;

        var hexPoints = _bentukDasar.GetHexagonVertices(Vector2.Zero, s);
        var rhombusPoints = _bentukDasar.GetBelahKetupatVertices(Vector2.Zero, h * 2, s);
        var trianglePoints = _bentukDasar.GetSegitigaSamaSisiVertices(Vector2.Zero, s);
        var trapPoints = _bentukDasar.GetTrapesiumSamaKakiVertices(Vector2.Zero, s, s * 2, h);

        // --- Susun Pola Penyiram Tanaman ---

        // Badan Utama (dari kiri ke kanan)
        // DrawComponent(trapPoints, Bentuk.ShapeType.TrapesiumSamaKaki, origin + new Vector2(-s * 2.25f, h/2), 0);
        DrawComponent(hexPoints, Bentuk.ShapeType.Hexagon, origin + new Vector2(-75, 0), 0);
        DrawComponent(hexPoints, Bentuk.ShapeType.Hexagon, origin + new Vector2(75, 0), 0);
        DrawComponent(trapPoints, Bentuk.ShapeType.TrapesiumSamaKaki, origin + new Vector2(0, -22), 0);
        DrawComponent(trapPoints, Bentuk.ShapeType.TrapesiumSamaKaki, origin + new Vector2(0, -66), 180);
        DrawComponent(rhombusPoints, Bentuk.ShapeType.BelahKetupat, origin + new Vector2(63, -67), 60);
        DrawComponent(rhombusPoints, Bentuk.ShapeType.BelahKetupat, origin + new Vector2(-63, -67), -60);
        DrawComponent(trianglePoints, Bentuk.ShapeType.SegitigaSamaSisi, origin + new Vector2(0, 16), 0);
        DrawComponent(trianglePoints, Bentuk.ShapeType.SegitigaSamaSisi, origin + new Vector2(28, 30), 180);
        DrawComponent(trianglePoints, Bentuk.ShapeType.SegitigaSamaSisi, origin + new Vector2(-26, 30), 180);


        // // Corong (Spout)
        DrawComponent(rhombusPoints, Bentuk.ShapeType.BelahKetupat, origin + new Vector2(125, 45), 0);
        DrawComponent(trianglePoints, Bentuk.ShapeType.SegitigaSamaSisi, origin + new Vector2(150, 75), 180);
        DrawComponent(trianglePoints, Bentuk.ShapeType.SegitigaSamaSisi, origin + new Vector2(150, 106), 0);
        DrawComponent(trapPoints, Bentuk.ShapeType.TrapesiumSamaKaki, origin + new Vector2(183, 125), -120);

        // // Gagang (Handle)
        DrawComponent(rhombusPoints, Bentuk.ShapeType.BelahKetupat, origin + new Vector2(63, 65), -60);
        DrawComponent(rhombusPoints, Bentuk.ShapeType.BelahKetupat, origin + new Vector2(-63, 65), 60);
        DrawComponent(rhombusPoints, Bentuk.ShapeType.BelahKetupat, origin + new Vector2(37, 110), -60);
        DrawComponent(rhombusPoints, Bentuk.ShapeType.BelahKetupat, origin + new Vector2(-37, 110), 60);
        DrawComponent(trianglePoints, Bentuk.ShapeType.SegitigaSamaSisi, origin + new Vector2(0, 103), 0);

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
        slot.TargetRotationDegrees = rotationDegrees;
        AddChild(slot);
        TotalSlots++;

        var collisionShape = new CollisionShape2D();
        var polygonShape = new ConvexPolygonShape2D();
        polygonShape.Points = worldPoints.ToArray();
        collisionShape.Shape = polygonShape;
        slot.AddChild(collisionShape);
    }

}
