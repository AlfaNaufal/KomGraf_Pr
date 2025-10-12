using Godot;
using System;
using System.Collections.Generic;

public partial class Bentuk : Area2D
{

    [Signal]
    public delegate void SpawnRequestedEventHandler(ShapeType type, Color color);

    // Variabel publik untuk diatur dari luar (oleh Play.cs)
    public bool IsTemplate { get; set; } = false;
    public ShapeType TypeToCreate { get; set; }
    public Color ColorToSet { get; set; }
    public Vector2 StartPosition { get; set; }

    public enum ShapeType { Hexagon, SegitigaSamaSisi, BelahKetupat }

    private Transformasi _transformasi = new Transformasi();
    private float[,] _matriksTransformasi = new float[3, 3];
    private List<Vector2> _originalVertices;
    private Color _shapeColor;
    private CollisionShape2D _collisionShape;
    private BentukDasar _bentukDasar = new BentukDasar();
    private bool _isDragging = false;
    private Vector2 _lastMousePosition;

    public override void _Ready()
    {
        // --- TAHAP 1: Persiapan Node ---
        _collisionShape = GetNode<CollisionShape2D>("CollisionShape2D");
        this.InputEvent += _on_InputEvent;

        // --- TAHAP 2: Inisialisasi Data & Matriks ---
        Transformasi.Matrix3x3Identity(_matriksTransformasi);
        _shapeColor = ColorToSet;

        // --- TAHAP 3: Buat Titik Sudut (Vertices) ---
        float side;
        switch (TypeToCreate)
        {
            case ShapeType.Hexagon:
                side = 2.5f * GameScale.PixelsPerCm;
                _originalVertices = _bentukDasar.GetHexagonVertices(Vector2.Zero, side);
                break;
            case ShapeType.SegitigaSamaSisi:
                side = 2.4f * GameScale.PixelsPerCm;
                _originalVertices = _bentukDasar.GetSegitigaSamaSisiVertices(Vector2.Zero, side);
                break;
            case ShapeType.BelahKetupat:
                side = 2.4f * GameScale.PixelsPerCm;
                _originalVertices = _bentukDasar.GetBelahKetupatVertices(Vector2.Zero, side);
                break;
        }

        // --- TAHAP 4: Terapkan Posisi Awal ---
        var dummyPivot = Vector2.Zero;
        _transformasi.Translation(_matriksTransformasi, StartPosition.X, StartPosition.Y, ref dummyPivot);
        
        // --- TAHAP 5: Gambar & Siapkan Kolisi untuk Pertama Kali ---
        ApplyTransformationAndDraw();
    }

    // SEMUA FUNGSI LAIN DI BAWAH INI TETAP SAMA
    private void _on_InputEvent(Node viewport, InputEvent @event, long shapeIdx)
    {
        // Hanya proses event tombol mouse kiri
        if (@event is InputEventMouseButton mouseButton && mouseButton.ButtonIndex == MouseButton.Left)
        {
            // Jika ini adalah templat dan mouse ditekan
            if (IsTemplate && mouseButton.Pressed)
            {
                EmitSignal(SignalName.SpawnRequested, (int)GetShapeTypeFromVertices(), Variant.From(_shapeColor));
                return; // Hentikan proses lebih lanjut untuk templat
            }

            // Jika ini bukan templat, atur status _isDragging berdasarkan apakah tombol sedang ditekan atau tidak
            _isDragging = mouseButton.Pressed;

            if (_isDragging)
            {
                // Aksi saat MULAI menyeret
                _lastMousePosition = KoordinatUtils.WorldToCartesian(GetGlobalMousePosition());
                this.ZIndex = 10;
            }
            else
            {
                // Aksi saat BERHENTI menyeret (mouse dilepas)
                this.ZIndex = 0;
                // Di sinilah nanti kita akan meletakkan logika SNAP
                GD.Print("Balok dilepas!"); 
            }
        }
    }

    private ShapeType GetShapeTypeFromVertices() {
        if (_originalVertices.Count == 6) return ShapeType.Hexagon;
        if (_originalVertices.Count == 4) return ShapeType.BelahKetupat;
        if (_originalVertices.Count == 3) return ShapeType.SegitigaSamaSisi;
        return ShapeType.Hexagon;
    }

    public override void _Input(InputEvent @event) {
        if (_isDragging && @event is InputEventKey eventKey && eventKey.Pressed) {
            float rotationAngle = 0;
            if (eventKey.Keycode == Key.Q) rotationAngle = 15;
            else if (eventKey.Keycode == Key.E) rotationAngle = -15;
            if (rotationAngle != 0) {
                var transformedVertices = _transformasi.GetTransformPoint(_matriksTransformasi, _originalVertices);
                var pivot = GetVerticesCenter(transformedVertices);
                var rotationMatrix = new float[3, 3];
                Transformasi.Matrix3x3Identity(rotationMatrix);
                _transformasi.RotationClockwise(rotationMatrix, rotationAngle, pivot);
                Transformasi.Matrix3x3Multiplication(rotationMatrix, _matriksTransformasi);
                ApplyTransformationAndDraw();
            }
        }
    }

    private Vector2 GetVerticesCenter(List<Vector2> vertices) {
        if (vertices == null || vertices.Count == 0) return Vector2.Zero;
        float sumX = 0; float sumY = 0;
        foreach (var v in vertices) { sumX += v.X; sumY += v.Y; }
        return new Vector2(sumX / vertices.Count, sumY / vertices.Count);
    }

    public override void _Process(double delta) {
        if (_isDragging) {
            var currentMousePosition = KoordinatUtils.WorldToCartesian(GetGlobalMousePosition());
            var mouseDelta = currentMousePosition - _lastMousePosition;
            if (mouseDelta.LengthSquared() > 0) {
                var translationMatrix = new float[3, 3];
                Transformasi.Matrix3x3Identity(translationMatrix);
                var dummyPivot = Vector2.Zero;
                _transformasi.Translation(translationMatrix, mouseDelta.X, mouseDelta.Y, ref dummyPivot);
                Transformasi.Matrix3x3Multiplication(translationMatrix, _matriksTransformasi);
                ApplyTransformationAndDraw();
            }
            _lastMousePosition = currentMousePosition;
        }
    }

    private void ApplyTransformationAndDraw() {
        var transformedVertices = _transformasi.GetTransformPoint(_matriksTransformasi, _originalVertices);
        var worldPointsForCollision = new List<Vector2>();
        foreach (var p in transformedVertices) {
            worldPointsForCollision.Add(KoordinatUtils.CartesianToWorld(p));
        }
        var collisionPolygon = new ConvexPolygonShape2D();
        collisionPolygon.Points = worldPointsForCollision.ToArray();
        _collisionShape.Shape = collisionPolygon;
        QueueRedraw();
    }

    public override void _Draw() {
        if (_originalVertices == null || _originalVertices.Count == 0) return;
        var transformedVertices = _transformasi.GetTransformPoint(_matriksTransformasi, _originalVertices);
        var worldPoints = new List<Vector2>();
        foreach (var p in transformedVertices) {
            worldPoints.Add(KoordinatUtils.CartesianToWorld(p));
        }
        DrawPolygon(worldPoints.ToArray(), new Color[] { _shapeColor });
    }

}
