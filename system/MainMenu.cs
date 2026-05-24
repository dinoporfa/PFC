using Godot;
using System;

public partial class MainMenu : Control
{
	GameManager gm;
	Button lastButton;
	public override void _Ready()
	{
		gm = GetTree().Root.GetNode<GameManager>("GameManager");
		GetNode<Button>("CenterContainer/VBoxContainer/Start").GrabFocus();
	}

	private void OnStartPressed()
	{
		gm.StartPlay("res://zones/void/Void.tscn", new Vector2(13*16, 12*16));
	}

	private void OnOptionsPressed()
	{
		lastButton = (Button)GetViewport().GuiGetFocusOwner();
	}

	private void OnExitPressed()
	{
		GetTree().Quit();
	}
}
