using Godot;
using System;
using System.Collections.Generic;

public partial class Play : Node2D
{
    
    public enum Level { AirHujan, Payung, PenyiramTanaman }
    private Level _currentLevel;
    // private Level _currentLevel = Level.AirHujan;
    // private Level _currentLevel = Level.Payung;
    // private Level _currentLevel = Level.PenyiramTanaman;

    private PackedScene _bentukScene = GD.Load<PackedScene>("res://Scenes/Bentuk.tscn");
    private OutlinePuzzle _outlinePuzzle;
    private List<Node> _levelObjects = new List<Node>(); // Untuk melacak semua objek level

    private int _totalSlotsInLevel = 0;
    private int _filledSlots = 0;

    private HUD _hud;
    private int _time = 0;

    public override void _Ready()
    {
        GameScale.Initialize(25.0f);
        var viewportSize = GetViewportRect().Size;
        KoordinatUtils.Initialize((int)viewportSize.X, (int)viewportSize.Y);

        _outlinePuzzle = GetNode<OutlinePuzzle>("OutlinePuzzle");
        _outlinePuzzle.OutlineDrawn += _on_Outline_Drawn;
        _outlinePuzzle.DrawLevel(_currentLevel);

        // _totalSlotsInLevel = _outlinePuzzle.TotalSlots; // Dapatkan total slot dari outline
        GD.Print("Total slot untuk level ini: " + _totalSlotsInLevel);

        CreatePalette();

        var hudScene = GD.Load<PackedScene>("res://Scenes/HUD.tscn");
        _hud = hudScene.Instantiate<HUD>();
        AddChild(_hud);

        // 2. Perbarui tampilan level di HUD
        _hud.UpdateLevel(_currentLevel.ToString());

        // _hud = GetNode<HUD>("HUD");
        
        // // Mulai level pertama
        // LoadLevel(_currentLevel);
    }

    private void _on_timer_timeout()
    {
        _time++;
        _hud.UpdateTime(_time);
    }
    
    private void _on_Outline_Drawn()
    {
        // Fungsi ini hanya akan berjalan SETELAH outline selesai digambar
        _totalSlotsInLevel = _outlinePuzzle.TotalSlots;
        foreach (var child in _outlinePuzzle.GetChildren())
        {
            if (child is Slot slot)
            {
                _levelObjects.Add(slot);
            }
        }
        GD.Print("Total slot untuk level ini (setelah digambar): " + _totalSlotsInLevel);
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
        _levelObjects.Add(templateBlock); // Templat adalah bagian dari level
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

        newBlock.BlockSnapped += _on_Block_Snapped; // Hubungkan sinyal dari balok ke fungsi kita
        newBlock.BlockUnsnapped += _on_Block_Unsnapped;

        AddChild(newBlock);

        _levelObjects.Add(newBlock); // Balok yang bisa digerakkan adalah bagian dari level
    }

    private void _on_Block_Snapped()
    {
        _filledSlots++;
        GD.Print($"Slot terisi: {_filledSlots} / {_totalSlotsInLevel}");

        if (_filledSlots >= _totalSlotsInLevel && _totalSlotsInLevel > 0)
        {
            GetNode<Timer>("Timer").Stop();
            _hud.ShowWinMessage();
            GD.Print("SELAMAT, ANDA MENANG!");

            // --- NAIK LEVEL SETELAH 2 DETIK ---
            var winTimer = GetTree().CreateTimer(2.0); // Buat timer tunggu 2 detik
            winTimer.Timeout += () => {
                // Sembunyikan pesan kemenangan lagi
                _hud.GetNode<Label>("%WinMessageLabel").Hide();

                // Tentukan level berikutnya (akan berputar kembali ke level 1 jika sudah tamat)
                int nextLevelIndex = ((int)_currentLevel + 1) % Enum.GetValues(typeof(Level)).Length;
                Level nextLevel = (Level)nextLevelIndex;

                LoadLevel(nextLevel);
            };
        }
    }

    private void _on_Block_Unsnapped()
    {
        if (_filledSlots > 0)
        {
            _filledSlots--;
        }
        GD.Print($"Balok diambil, slot terisi: {_filledSlots} / {_totalSlotsInLevel}");
    }

    private void LoadLevel(Level level)
    {
        // Hapus semua objek dari level sebelumnya
        foreach (var obj in _levelObjects)
        {
            obj.QueueFree();
        }
        _levelObjects.Clear();

        // Reset state
        _currentLevel = level;
        _filledSlots = 0;
        _time = 0;
        _hud.UpdateTime(0);
        GetNode<Timer>("Timer").Start();
        
        _outlinePuzzle.DrawLevel(_currentLevel);
        _hud.UpdateLevel(_currentLevel.ToString());
        
        CreatePalette();
    }

}
