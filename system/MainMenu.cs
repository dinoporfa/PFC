using Godot;
using System;

public partial class MainMenu : Control
{
	GameManager gm;
	public override void _Ready()
	{
		gm = GetTree().Root.GetNode<GameManager>("GameManager");
		GetNode<Player>("Player").CanMove(false);
	}

	private void OnStartPressed()
	{
		gm.ChangeScene("res://zones/void/Void.tscn", new Vector2(13*16, 12*16), "down");
	}

	private void OnOptionsPressed()
	{
		
	}

	private void OnExitPressed()
	{
		GetTree().Quit();
	}
}
