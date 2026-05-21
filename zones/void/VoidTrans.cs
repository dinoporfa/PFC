using Godot;
using System;

public partial class VoidTrans : Area2D
{
	GameManager gm;
	Player player;
    public override void _Ready()
    {
        gm = GetTree().Root.GetNode<GameManager>("GameManager");
		player = GetTree().Root.GetNode<Player>(gm.currentScene.Name + "/Player");
    }

	private void VoidToPuzzle1(Node2D body)
	{
		if (body == player)
		{
			gm.ChangeScene("res://zones/void/Puzzle1.tscn", new Vector2(12*16, 15*16) , "up");
		}
	}

	private void Puzzle1ToVoid(Node2D body)
	{
		if (body == player)
		{
			gm.ChangeScene("res://zones/void/Void.tscn", new Vector2(135*16, 1*16) , "down");
		}
	}
}