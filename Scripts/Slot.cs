using Godot;

public partial class Slot : Area2D
{
    // Menyimpan tipe bentuk apa yang cocok untuk slot ini
    public Bentuk.ShapeType TargetShape { get; set; }

    // Menyimpan transformasi (posisi & rotasi) yang benar untuk slot ini
    public float[,] TargetMatrix { get; set; } = new float[3, 3];
}