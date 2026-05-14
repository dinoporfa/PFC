using Godot;
using System;

public partial class CourtneyAppears : Area2D
{
	CharacterBody2D player;
	Dialogic dialogic;
	public override void _Ready()
	{
		dialogic = new Dialogic(GetNode<Node>("DialogicEditor"));
		player = GetNode<CharacterBody2D>("/root/Void/Player");
	}
	private void CourtneyAppears1(Node2D body)
	{
		if (body == player)
		{
			dialogic.Start("Courtney1");
		}
	}
	private void CourtneyAppears2(Node2D body)
	{
		if (body == player)
		{
			dialogic.Start("Courtney2");
		}
	}
	private void CourtneyAppears3(Node2D body)
	{
		if (body == player)
		{
			dialogic.Start("Courtney3");
		}
	}
	private void CourtneyAppears4(Node2D body)
	{
		if (body == player)
		{
			dialogic.Start("Courtney4");
		}
	}
}
