using Godot;
using System;
using System.Collections.Generic;

public partial class Bentuk : Area2D
{

    private Slot _overlappingSlot = null;
    private bool _isLocked = false;

    [Signal]
    public delegate void SpawnRequestedEventHandler(ShapeType type, Color color);

    public bool IsTemplate { get; set; } = false;
    public ShapeType TypeToCreate { get; set; }
    public Color ColorToSet { get; set; }
    public Vector2 StartPosition { get; set; }

    public enum ShapeType { 
        Hexagon, SegitigaSamaSisi, BelahKetupat, Persegi, PersegiPanjang, 
        SegitigaSiku, TrapesiumSiku, TrapesiumSamaKaki, JajarGenjang
    }

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
        _collisionShape = GetNode<CollisionShape2D>("CollisionShape2D");
        this.InputEvent += _on_InputEvent;

        Transformasi.Matrix3x3Identity(_matriksTransformasi);
        _shapeColor = ColorToSet;

        float sisiDasar = 2.5f * GameScale.PixelsPerCm; // Sisi hexagon kuning sebagai acuan (2.5 cm)

        switch (TypeToCreate)
        {
            case ShapeType.Hexagon:
                // Sisi hexagon = sisi persegi
                _originalVertices = _bentukDasar.GetHexagonVertices(Vector2.Zero, 2.0f * GameScale.PixelsPerCm);
                break;
            case ShapeType.SegitigaSamaSisi:
                _originalVertices = _bentukDasar.GetSegitigaSamaSisiVertices(Vector2.Zero, 2.0f * GameScale.PixelsPerCm);
                break;
            case ShapeType.BelahKetupat:
                // Diagonal pendek = sisi persegi, Diagonal panjang = tinggi 2 segitiga
                _originalVertices = _bentukDasar.GetBelahKetupatVertices(Vector2.Zero, (2.0f * Mathf.Sqrt(3)) * GameScale.PixelsPerCm, 2.0f * GameScale.PixelsPerCm);
                break;
            case ShapeType.Persegi:
                _originalVertices = _bentukDasar.GetPersegiVertices(Vector2.Zero, 2.0f * GameScale.PixelsPerCm);
                break;
            case ShapeType.PersegiPanjang:
                _originalVertices = _bentukDasar.GetPersegiPanjangVertices(Vector2.Zero, 4.0f * GameScale.PixelsPerCm, 2.0f * GameScale.PixelsPerCm);
                break;
            case ShapeType.TrapesiumSamaKaki:
                // Sisi atas = 2cm, Sisi bawah = 4cm
                _originalVertices = _bentukDasar.GetTrapesiumSamaKakiVertices(Vector2.Zero, 2.0f * GameScale.PixelsPerCm, 4.0f * GameScale.PixelsPerCm, (2.0f * Mathf.Sqrt(3) / 2) * GameScale.PixelsPerCm);
                break;
            case ShapeType.JajarGenjang:
                // Alas = 2cm
                _originalVertices = _bentukDasar.GetJajarGenjangVertices(Vector2.Zero, 2.0f * GameScale.PixelsPerCm, (2.0f * Mathf.Sqrt(3) / 2) * GameScale.PixelsPerCm, 1.0f * GameScale.PixelsPerCm);
                break;

            // Bentuk non-standar lainnya kita buat proporsional dengan sisi 2cm
            case ShapeType.SegitigaSiku:
                _originalVertices = _bentukDasar.GetSegitigaSikuVertices(Vector2.Zero, 2.0f * GameScale.PixelsPerCm, 2.0f * GameScale.PixelsPerCm);
                break;
            case ShapeType.TrapesiumSiku:
                _originalVertices = _bentukDasar.GetTrapesiumSikuVertices(Vector2.Zero, 2.0f * GameScale.PixelsPerCm, 4.0f * GameScale.PixelsPerCm, 2.0f * GameScale.PixelsPerCm);
                break;
        }

        var dummyPivot = Vector2.Zero;
        _transformasi.Translation(_matriksTransformasi, StartPosition.X, StartPosition.Y, ref dummyPivot);

        ApplyTransformationAndDraw();
    }

    private void _on_area_entered(Area2D area)
    {
        if (area is Slot slot)
        {
            _overlappingSlot = slot;
        }
    }

    private void _on_area_exited(Area2D area)
    {
        if (area == _overlappingSlot)
        {
            _overlappingSlot = null;
        }
    }
    
    private void _on_InputEvent(Node viewport, InputEvent @event, long shapeIdx)
    {
        if (_isLocked) return; // Jika sudah terkunci, jangan lakukan apa-apa

        if (@event is InputEventMouseButton mouseButton && mouseButton.ButtonIndex == MouseButton.Left)
        {
            if (IsTemplate && mouseButton.Pressed)
            {
                EmitSignal(SignalName.SpawnRequested, (int)TypeToCreate, Variant.From(_shapeColor));
                return;
            }

            _isDragging = mouseButton.Pressed;

            if (_isDragging)
            {
                _lastMousePosition = KoordinatUtils.WorldToCartesian(GetGlobalMousePosition());
                this.ZIndex = 10;
            }
            else // Ini dieksekusi saat mouse dilepas
            {
                this.ZIndex = 0;

                // --- LOGIKA SNAP ---
                if (_overlappingSlot != null && _overlappingSlot.TargetShape == this.TypeToCreate)
                {
                    // Kunci jawaban cocok!
                    _matriksTransformasi = (float[,])_overlappingSlot.TargetMatrix.Clone();
                    ApplyTransformationAndDraw();
                    _isLocked = true; // Kunci balok
                    GD.Print("Snap berhasil untuk " + _overlappingSlot.Name);
                }
            }
        }
    }
    
    // private void _on_InputEvent(Node viewport, InputEvent @event, long shapeIdx)
    // {
    //     if (@event is InputEventMouseButton mouseButton && mouseButton.ButtonIndex == MouseButton.Left)
    //     {
    //         if (IsTemplate)
    //         {
    //             if (mouseButton.Pressed) // Hanya spawn saat mouse ditekan
    //             {
    //                 EmitSignal(SignalName.SpawnRequested, (int)TypeToCreate, Variant.From(_shapeColor));
    //             }
    //             return;
    //         }

    //         // Logika untuk balok yang bisa digerakkan
    //         if (mouseButton.Pressed)
    //         {
    //             _isDragging = true;
    //             _lastMousePosition = KoordinatUtils.WorldToCartesian(GetGlobalMousePosition());
    //             this.ZIndex = 10;
    //         }
    //         else // Ini hanya akan tereksekusi saat mouse dilepas
    //         {
    //             _isDragging = false;
    //             this.ZIndex = 0;
    //         }
    //     }
    // }

    public override void _Input(InputEvent @event) {
        if (_isDragging && @event is InputEventKey eventKey && eventKey.Pressed) {
            float rotationAngle = 0;
            if (eventKey.Keycode == Key.E) rotationAngle = 15;
            else if (eventKey.Keycode == Key.Q) rotationAngle = -15;
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

    private Vector2 GetVerticesCenter(List<Vector2> vertices) {
        if (vertices == null || vertices.Count == 0) return Vector2.Zero;
        float sumX = 0; float sumY = 0;
        foreach (var v in vertices) { sumX += v.X; sumY += v.Y; }
        return new Vector2(sumX / vertices.Count, sumY / vertices.Count);
    }
    
    private void ApplyTransformationAndDraw() {
        if (_originalVertices == null) return;
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
