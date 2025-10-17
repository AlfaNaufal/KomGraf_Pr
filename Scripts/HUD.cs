using Godot;
using System;

public partial class HUD : CanvasLayer
{

	[Signal] public delegate void NextLevelRequestedEventHandler();
	[Signal] public delegate void MainMenuRequestedEventHandler();
	
	private Label _timeLabel;
	private Label _levelLabel;
	private VBoxContainer _winContainer;
	private HBoxContainer _winButtonContainer;
	private Label _winMessageLabel;
	private Button _nextLevelButton;
	private Button _menuButton;

	public override void _Ready()
	{
		_timeLabel = GetNode<Label>("MarginContainer/VBoxContainer/Waktu");
		_levelLabel = GetNode<Label>("MarginContainer/VBoxContainer/Level");
		_winContainer = GetNode<VBoxContainer>("MarginContainer/WinContainer");
		_winMessageLabel = GetNode<Label>("MarginContainer/WinContainer/WinMessageLabel");
		_winButtonContainer = GetNode<HBoxContainer>("MarginContainer/WinContainer/WinButtonContainer");
		_nextLevelButton = GetNode<Button>("MarginContainer/WinContainer/WinButtonContainer/NextLevelButton");
		_menuButton = GetNode<Button>("MarginContainer/WinContainer/WinButtonContainer/MenuButton");
		
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
		_winButtonContainer.Show();
		_winMessageLabel.Show();
		_nextLevelButton.Show();
		_menuButton.Show();
	}

	public void HideWinUI()
	{
		_winContainer.Hide();
		_winButtonContainer.Hide();
		_winMessageLabel.Hide();
		_nextLevelButton.Hide();
		_menuButton.Hide();
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
