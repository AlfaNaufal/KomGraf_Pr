using Godot;
using System;

public partial class HUD : CanvasLayer
{

	private Label _timeLabel;
	private Label _levelLabel;
	private Label _winMessageLabel;

	public override void _Ready()
	{
		_timeLabel = GetNode<Label>("MarginContainer/VBoxContainer/Waktu");
		_levelLabel = GetNode<Label>("MarginContainer/VBoxContainer/Level");
		_winMessageLabel = GetNode<Label>("MarginContainer/VBoxContainer/WinMessageLabel");
	}

	public void UpdateTime(int time)
	{
		_timeLabel.Text = $"Waktu: {time}";
	}

	public void UpdateLevel(string levelName)
	{
		_levelLabel.Text = $"Level: {levelName}";
	}

	public void ShowWinMessage()
	{
		_winMessageLabel.Show();
	}

}
