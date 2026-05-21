using Godot;
using System;

public partial class VoidDialog : Node2D
{
	GameManager gm;
	Player player;
	public override void _Ready()
	{
		gm = GetTree().Root.GetNode<GameManager>("GameManager");
		player = GetTree().Root.GetNode<Player>(gm.currentScene.Name + "/Player");
	}

	private void CourtneyAppears1(Node2D body)
	{
		if (body == player)
		{
			GD.Print("ola");
		}
	}

	private void CourtneyAppears2(Node2D body)
	{
		if (body == player)
		{

		}
	}

	private void CourtneyAppears3(Node2D body)
	{
		if (body == player)
		{

		}
	}

	private void CourtneyAppears4(Node2D body)
	{
		if (body == player)
		{

		}
	}
}