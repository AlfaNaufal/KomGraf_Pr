using Godot;
using System;

public partial class HUD : CanvasLayer
{

    private Label _timeLabel;
    private Label _levelLabel;

    public override void _Ready()
    {
        _timeLabel = GetNode<Label>("MarginContainer/VBoxContainer/TimeLabel");
        _levelLabel = GetNode<Label>("MarginContainer/VBoxContainer/LevelLabel");
    }

    public void UpdateTime(int time)
    {
        _timeLabel.Text = $"Waktu: {time}";
    }

    public void UpdateLevel(string levelName)
    {
        _levelLabel.Text = $"Level: {levelName}";
    }

}
