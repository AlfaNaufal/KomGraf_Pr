using Godot;
using System;

public partial class Play : Node2D
{
    
    public enum Level { AirHujan, Payung, PenyiramTanaman }
    private Level _currentLevel = Level.AirHujan;

    private PackedScene _bentukScene = GD.Load<PackedScene>("res://Scenes/Bentuk.tscn");
    private OutlinePuzzle _outlinePuzzle;

    public override void _Ready()
    {
        GameScale.Initialize(25.0f);
        var viewportSize = GetViewportRect().Size;
        KoordinatUtils.Initialize((int)viewportSize.X, (int)viewportSize.Y);

        _outlinePuzzle = GetNode<OutlinePuzzle>("OutlinePuzzle");
        _outlinePuzzle.DrawLevel(_currentLevel);
        
        CreatePalette();
    }

    private void CreatePalette()
    {
        Vector2 startPos = new Vector2(-550, 280);
        float spacingY = 110; // Beri sedikit jarak lebih
        float spacingX = 120;
        int col = 0;
        int row = 0;

        Vector2 nextPos() {
            var pos = startPos + new Vector2(col * spacingX, -row * spacingY);
            row++;
            if (row > 4) {
                row = 0;
                col++;
            }
            return pos;
        }

        // Tambahkan bentuk-bentuk dari daftar baru Anda ke palet
        AddTemplateToPalette(Bentuk.ShapeType.Persegi, Colors.Orange, nextPos());
        AddTemplateToPalette(Bentuk.ShapeType.PersegiPanjang, Colors.LightBlue, nextPos());
        AddTemplateToPalette(Bentuk.ShapeType.SegitigaSamaSisi, Colors.Green, nextPos());
        AddTemplateToPalette(Bentuk.ShapeType.TrapesiumSamaKaki, Colors.Red, nextPos());
        AddTemplateToPalette(Bentuk.ShapeType.Hexagon, Colors.Yellow, nextPos());
        AddTemplateToPalette(Bentuk.ShapeType.BelahKetupat, Colors.Blue, nextPos());
        AddTemplateToPalette(Bentuk.ShapeType.JajarGenjang, Colors.Tan, nextPos());
    }

    private void AddTemplateToPalette(Bentuk.ShapeType type, Color color, Vector2 position)
    {
        var templateBlock = _bentukScene.Instantiate<Bentuk>();
        templateBlock.IsTemplate = true;
        templateBlock.TypeToCreate = type;
        templateBlock.ColorToSet = color;
        templateBlock.StartPosition = position;
        templateBlock.SpawnRequested += OnSpawnRequested;
        AddChild(templateBlock);
    }
    
    private void OnSpawnRequested(Bentuk.ShapeType type, Color color)
    {
        SpawnBlock(type, color, Vector2.Zero);
    }

    private void SpawnBlock(Bentuk.ShapeType type, Color color, Vector2 position)
    {
        var newBlock = _bentukScene.Instantiate<Bentuk>();
        newBlock.IsTemplate = false;
        newBlock.TypeToCreate = type;
        newBlock.ColorToSet = color;
        newBlock.StartPosition = position;
        AddChild(newBlock);
    }

}
