using Godot;

public partial class Slot : Area2D
{
    public Bentuk.ShapeType TargetShape { get; set; }
    public float[,] TargetMatrix { get; set; } = new float[3, 3];

    public float TargetRotationDegrees { get; set; } = 0;
    public bool IsFilled { get; set; } = false;
}