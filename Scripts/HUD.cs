using Godot;
using System;

public partial class HUD : CanvasLayer
{

	[Signal] public delegate void NextLevelRequestedEventHandler();
	[Signal] public delegate void MainMenuRequestedEventHandler();
	
	private Label _timeLabel;
	private Label _levelLabel;
	private HBoxContainer _winContainer;
	private Button _nextLevelButton;
	private Button _menuButton;

	public override void _Ready()
	{
		_timeLabel = GetNode<Label>("MarginContainer/VBoxContainer/Waktu");
		_levelLabel = GetNode<Label>("MarginContainer/VBoxContainer/Level");
		_winContainer = GetNode<HBoxContainer>("MarginContainer/WinButtonContainer");
		_nextLevelButton = GetNode<Button>("MarginContainer/WinButtonContainer/NextLevelButton");
		_menuButton = GetNode<Button>("MarginContainer/WinButtonContainer/MenuButton");
		
		// Hubungkan sinyal tombol ke fungsi internal
		_nextLevelButton.Pressed += OnNextLevelPressed;
		_menuButton.Pressed += OnMenuPressed;
	}

	public void UpdateTime(int time)
	{
		_timeLabel.Text = $"Waktu: {time}";
	}

	public void UpdateLevel(string levelName)
	{
		_levelLabel.Text = $"Level: {levelName}";
	}

	//public void ShowWinMessage()
	//{
		//_winMessageLabel.Show();
	//}
	
	public void ShowWinUI()
	{
		_winContainer.Show();
	}

	public void HideWinUI()
	{
		_winContainer.Hide();
	}

	// Fungsi internal yang akan memancarkan sinyal keluar
	private void OnNextLevelPressed()
	{
		EmitSignal(SignalName.NextLevelRequested);
	}

	private void OnMenuPressed()
	{
		EmitSignal(SignalName.MainMenuRequested);
	}

}
