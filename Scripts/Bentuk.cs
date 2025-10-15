using Godot;
using System;
using System.Collections.Generic;

public partial class Bentuk : Area2D
{

    [Signal] public delegate void BlockSnappedEventHandler();
    [Signal] public delegate void BlockUnsnappedEventHandler();
    [Signal] public delegate void SpawnRequestedEventHandler(ShapeType type, Color color);

    private Slot _overlappingSlot = null;
    // private bool _isLocked = false;
    private Slot _snappedToSlot = null; // Untuk mengingat slot mana yang kita tempati
    private float _currentRotationDegrees = 0;
    private const float SNAP_DISTANCE_THRESHOLD = 75.0f; // Toleransi jarak dalam piksel
    // private const float SNAP_ROTATION_THRESHOLD = 25.0f; // Toleransi rotasi dalam derajat

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
        this.AreaEntered += _on_area_entered;
        this.AreaExited += _on_area_exited;

        Transformasi.Matrix3x3Identity(_matriksTransformasi);
        _shapeColor = ColorToSet;

        switch (TypeToCreate)
        {
            case ShapeType.Hexagon:
                _originalVertices = _bentukDasar.GetHexagonVertices(Vector2.Zero, 2.0f * GameScale.PixelsPerCm);
                break;
            case ShapeType.SegitigaSamaSisi:
                _originalVertices = _bentukDasar.GetSegitigaSamaSisiVertices(Vector2.Zero, 2.0f * GameScale.PixelsPerCm);
                break;
            case ShapeType.BelahKetupat:
                _originalVertices = _bentukDasar.GetBelahKetupatVertices(Vector2.Zero, (2.0f * Mathf.Sqrt(3)) * GameScale.PixelsPerCm, 2.0f * GameScale.PixelsPerCm);
                break;
            case ShapeType.Persegi:
                _originalVertices = _bentukDasar.GetPersegiVertices(Vector2.Zero, 2.0f * GameScale.PixelsPerCm);
                break;
            case ShapeType.PersegiPanjang:
                _originalVertices = _bentukDasar.GetPersegiPanjangVertices(Vector2.Zero, 4.0f * GameScale.PixelsPerCm, 2.0f * GameScale.PixelsPerCm);
                break;
            case ShapeType.TrapesiumSamaKaki:
                _originalVertices = _bentukDasar.GetTrapesiumSamaKakiVertices(Vector2.Zero, 2.0f * GameScale.PixelsPerCm, 4.0f * GameScale.PixelsPerCm, (2.0f * Mathf.Sqrt(3) / 2) * GameScale.PixelsPerCm);
                break;
            case ShapeType.JajarGenjang:
                _originalVertices = _bentukDasar.GetJajarGenjangVertices(Vector2.Zero, 2.0f * GameScale.PixelsPerCm, (2.0f * Mathf.Sqrt(3) / 2) * GameScale.PixelsPerCm, 1.0f * GameScale.PixelsPerCm);
                break;
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
        
        }else if (area.Name == "TrashArea"){

            this.Modulate = new Color(1, 1, 1, 0.5f);
        }
    }

    private void _on_area_exited(Area2D area)
    {
        if (area == _overlappingSlot)
        {
            _overlappingSlot = null;

        }else if (area.Name == "TrashArea"){
            this.Modulate = new Color(1, 1, 1, 1);
        }
    }
    
    private void _on_InputEvent(Node viewport, InputEvent @event, long shapeIdx)
    {
        // Fungsi ini sekarang HANYA untuk memulai DRAG
        if (@event is InputEventMouseButton mouseButton && mouseButton.ButtonIndex == MouseButton.Left && mouseButton.Pressed)
        {
            if (IsTemplate)
            {
                EmitSignal(SignalName.SpawnRequested, (int)TypeToCreate, Variant.From(_shapeColor));
                return;
            }
            
            // Kosongkan slot jika kita mengambil balok yang sudah di-snap
            if (_snappedToSlot != null)
            {
                _snappedToSlot.IsFilled = false;
                _snappedToSlot = null;
                EmitSignal(SignalName.BlockUnsnapped);
            }

            _isDragging = true;
            _lastMousePosition = KoordinatUtils.WorldToCartesian(GetGlobalMousePosition());
            this.ZIndex = 10;
            GetViewport().SetInputAsHandled();
        }
        else{
            // --- LOGIKA SNAP YANG DIPERBARUI ---
            if (_overlappingSlot != null && !_overlappingSlot.IsFilled && _overlappingSlot.TargetShape == this.TypeToCreate)
            {
                // Dapatkan pusat balok saat ini & pusat slot target
                var currentVertices = _transformasi.GetTransformPoint(_matriksTransformasi, _originalVertices);
                var currentCenter = GetVerticesCenter(currentVertices);
                var targetMatrix = _overlappingSlot.TargetMatrix;
                var targetCenter = new Vector2(targetMatrix[0, 2], targetMatrix[1, 2]);

                float distance = currentCenter.DistanceTo(targetCenter);
                
                // HANYA CEK JARAK, TIDAK PERLU CEK ROTASI
                if (distance < SNAP_DISTANCE_THRESHOLD)
                {
                    // Kunci jawaban cocok!
                    _matriksTransformasi = (float[,])_overlappingSlot.TargetMatrix.Clone();
                    ApplyTransformationAndDraw();
                    
                    _overlappingSlot.IsFilled = true; // Tandai slot ini sudah terisi
                    _snappedToSlot = _overlappingSlot; // Ingat kita menempel di sini
                    GD.Print("Snap berhasil untuk " + _overlappingSlot.Name);

                    EmitSignal(SignalName.BlockSnapped); // Kirim sinyal kemenangan!
                }
            }
        }
    }

    public override void _Input(InputEvent @event) {
        // --- Bagian 1: Logika Melepas Mouse (Global) ---
        if (@event is InputEventMouseButton mouseButton && mouseButton.ButtonIndex == MouseButton.Left && !mouseButton.Pressed)
        {
            if (_isDragging)
            {
                _isDragging = false;
                this.ZIndex = 0;

                // Cek apakah balok dilepas di atas area sampah
                var trashArea = GetNode<Area2D>("%TrashArea");
                if (this.OverlapsArea(trashArea))
                {
                    // Hapus diri sendiri
                    QueueFree();
                    return; // Hentikan agar tidak menjalankan logika snap
                }

                // --- LOGIKA SNAP ---
                if (_overlappingSlot != null && !_overlappingSlot.IsFilled && _overlappingSlot.TargetShape == this.TypeToCreate)
                {
                    var currentVertices = _transformasi.GetTransformPoint(_matriksTransformasi, _originalVertices);
                    var currentCenter = GetVerticesCenter(currentVertices);
                    var targetMatrix = _overlappingSlot.TargetMatrix;
                    var targetCenter = new Vector2(targetMatrix[0, 2], targetMatrix[1, 2]);
                    float distance = currentCenter.DistanceTo(targetCenter);

                    if (distance < SNAP_DISTANCE_THRESHOLD)
                    {
                        _matriksTransformasi = (float[,])_overlappingSlot.TargetMatrix.Clone();
                        ApplyTransformationAndDraw();
                        _snappedToSlot = _overlappingSlot;
                        _snappedToSlot.IsFilled = true;
                        GD.Print("Snap berhasil untuk " + _overlappingSlot.Name);
                    }
                }
            }
        }
    }

    public override void _Process(double delta) {
        if (_isDragging) 
        {
            // --- LOGIKA ROTASI BARU (DIPINDAHKAN KE SINI) ---
            float rotationChange = 0;
            if (Input.IsKeyPressed(Key.E)) rotationChange = 15;
            else if (Input.IsKeyPressed(Key.Q)) rotationChange = -15;
            
            if (rotationChange != 0)
            {
                // Terapkan rotasi. Kita kalikan dengan delta agar kecepatan rotasi konsisten.
                float angleToRotate = rotationChange * (float)delta * 5.0f; // Angka 5.0f adalah pengali kecepatan, bisa disesuaikan
                _currentRotationDegrees += angleToRotate;
                _currentRotationDegrees = (_currentRotationDegrees % 360 + 360) % 360;

                var transformedVertices = _transformasi.GetTransformPoint(_matriksTransformasi, _originalVertices);
                var pivot = GetVerticesCenter(transformedVertices);
                var rotationMatrix = new float[3, 3];
                Transformasi.Matrix3x3Identity(rotationMatrix);
                _transformasi.RotationClockwise(rotationMatrix, angleToRotate, pivot);
                Transformasi.Matrix3x3Multiplication(rotationMatrix, _matriksTransformasi);
            }

            // --- LOGIKA MENGGESER (TETAP SAMA) ---
            var currentMousePosition = KoordinatUtils.WorldToCartesian(GetGlobalMousePosition());
            var mouseDelta = currentMousePosition - _lastMousePosition;
            if (mouseDelta.LengthSquared() > 0) {
                var translationMatrix = new float[3, 3];
                Transformasi.Matrix3x3Identity(translationMatrix);
                var dummyPivot = Vector2.Zero;
                _transformasi.Translation(translationMatrix, mouseDelta.X, mouseDelta.Y, ref dummyPivot);
                Transformasi.Matrix3x3Multiplication(translationMatrix, _matriksTransformasi);
            }
            
            ApplyTransformationAndDraw();
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
