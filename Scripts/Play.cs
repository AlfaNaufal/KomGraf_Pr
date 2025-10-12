using Godot;
using System;

public partial class Play : Node2D
{
    
    // // Muat scene blueprint balok kita
    // private PackedScene _bentukScene = GD.Load<PackedScene>("res://Scenes/Bentuk.tscn");

    // public override void _Ready()
    // {
    //     var viewportSize = GetViewportRect().Size;
    //     KoordinatUtils.Initialize((int)viewportSize.X, (int)viewportSize.Y);
        
    //     // Buat "Palet" atau tempat menaruh balok di bagian bawah layar
    //     SpawnBlock(Bentuk.ShapeType.Hexagon, Colors.Yellow, new Vector2(-200, -250));
    //     SpawnBlock(Bentuk.ShapeType.SegitigaSamaSisi, Colors.Green, new Vector2(-100, -250));
    //     SpawnBlock(Bentuk.ShapeType.BelahKetupat, Colors.Blue, new Vector2(0, -250));
    // }

    // private void SpawnBlock(Bentuk.ShapeType type, Color color, Vector2 position)
    // {
    //     var newBlock = _bentukScene.Instantiate<Bentuk>();
    //     AddChild(newBlock);
    //     // Panggil fungsi inisialisasi yang kita buat
    //     newBlock.Initialize(type, color, position);
    // }

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
        Vector2 paletteStartPosition = new Vector2(-550, 250);
        float spacing = 100;

        AddTemplateToPalette(Bentuk.ShapeType.Hexagon, Colors.Yellow, paletteStartPosition);
        AddTemplateToPalette(Bentuk.ShapeType.SegitigaSamaSisi, Colors.Green, paletteStartPosition + new Vector2(0, -spacing));
        AddTemplateToPalette(Bentuk.ShapeType.BelahKetupat, Colors.Blue, paletteStartPosition + new Vector2(0, -spacing * 2));
    }

    private void AddTemplateToPalette(Bentuk.ShapeType type, Color color, Vector2 position)
    {
        var templateBlock = _bentukScene.Instantiate<Bentuk>();
        // Atur semua data SEBELUM menambahkannya ke scene
        templateBlock.IsTemplate = true;
        templateBlock.TypeToCreate = type;
        templateBlock.ColorToSet = color;
        templateBlock.StartPosition = position;
        templateBlock.SpawnRequested += OnSpawnRequested;
        // Sekarang baru tambahkan ke scene. Godot akan memanggil _Ready() secara otomatis.
        AddChild(templateBlock);
    }
    
    private void OnSpawnRequested(Bentuk.ShapeType type, Color color)
    {
        // var mousePos = KoordinatUtils.WorldToCartesian(GetGlobalMousePosition());
        SpawnBlock(type, color, Vector2.Zero);
    }

    private void SpawnBlock(Bentuk.ShapeType type, Color color, Vector2 position)
    {
        var newBlock = _bentukScene.Instantiate<Bentuk>();
        newBlock.IsTemplate = false; // Ini bukan templat
        newBlock.TypeToCreate = type;
        newBlock.ColorToSet = color;
        newBlock.StartPosition = position;
        // Sekarang baru tambahkan ke scene. Godot akan memanggil _Ready() secara otomatis.
        AddChild(newBlock);
    }

}
