using Godot;
using System;

public partial class VoidTrans : Area2D
{
	GameManager gm;
	CharacterBody2D player;
	
	public override void _Ready()
    {
        gm = GetNode<GameManager>("/root/GameManager");
		player = GetNode<CharacterBody2D>("/root/Void/Player");
    }

	private void VoidToPuzzle1(Node body)
	{
		if (body == player)
			gm.ChangeScene("res://scenes/void/VoidPuzzle1.tscn");
	}
}
