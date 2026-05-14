using Godot;
using System;

public partial class MainMenu : Control
{
	GameManager gm;

    public override void _Ready()
    {
        gm = GetNode<GameManager>("/root/GameManager");
    }
	private void OnStartPressed()
	{
		gm.ChangeScene("res://scenes/void/void.tscn");
	}

	private void OnExitPressed()
	{
		GetTree().Quit();
	}
}
