using Godot;
using System;

public partial class VoidDialog : Area2D
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
