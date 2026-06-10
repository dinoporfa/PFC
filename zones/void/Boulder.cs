using Godot;
using System;

public partial class Boulder : CharacterBody2D
{
	const int tileSize = 16;
	Vector2 inputDir;
	RayCast2D playerRay, objectRay;
	public override void _Ready()
	{
		playerRay = GetTree().Root.GetNode<RayCast2D>(GetParent().Name + "/Player/RayCast2D");
		objectRay = GetNode<RayCast2D>("RayCast2D");
	}

	public override void _Process(double delta)
	{
		if(playerRay.GetCollider() == this)
		{
			if (Player.GetPlayerInputDir() != Vector2.Zero)
				inputDir = Player.GetPlayerInputDir();
			if (Input.IsActionJustPressed("a"))
			{
				objectRay.TargetPosition = inputDir * tileSize/2;
				objectRay.ForceRaycastUpdate();
				if (!objectRay.IsColliding())
				{
					GlobalPosition += inputDir * tileSize;
				}
			}
		}
	}
}
