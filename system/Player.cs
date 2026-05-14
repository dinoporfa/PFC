using Godot;
using System;

public partial class Player : CharacterBody2D
{
	
	int speed = 4;
	const int tileSize = 16;
	Vector2 initialPosition;
	static Vector2 inputDir;
	bool isMoving;
	float percentMoved;
	AnimationTree animationTree;
	static RayCast2D ray;


	public static Vector2 GetInputDir()
	{
		return inputDir;
	}
	public override void _Ready()
	{
		initialPosition = Position;
		animationTree = GetNode<AnimationTree>("AnimationTree");
		animationTree.Active = true;
		animationTree.Set("parameters/Idle/blend_position", new Vector2(0, 1));
		ray = GetNode<RayCast2D>("RayCast2D");
	}

	public override void _Process(double delta)
	{
		
	}

	public override void _PhysicsProcess(double delta)
	{
		HandleAnimation();
		if (!isMoving)
		{
			ProcessInput();
		}
		else if (inputDir != Vector2.Zero)
		{
			Move(delta);
		}
		else
		{
			isMoving = false;
		}
	}

	private void ProcessInput()
	{
		if (inputDir.Y == 0)
		{
			// nano porfi explica
			inputDir.X = Input.IsActionPressed("right") ? 1 : Input.IsActionPressed("left") ? -1 : 0;
			/*estos son equivalentes(?
			if (Input.IsActionPressed("right"))
			{
				inputDir.X = 1;
			}
			else if (Input.IsActionPressed("left"))
			{
				inputDir.X = -1;
			}
			else
			{
				inputDir.X = 0;
			}*/
		}
		if (inputDir.X == 0)
		{
			inputDir.Y = Input.IsActionPressed("down") ? 1 : Input.IsActionPressed("up") ? -1 : 0;
			/*if (Input.IsActionPressed("down"))
			{
				inputDir.Y = 1;
			}
			else if (Input.IsActionPressed("up"))
			{
				inputDir.Y = -1;
			}
			else
			{
				inputDir.Y = 0;
			}*/
		}
		if (inputDir != Vector2.Zero)
		{
			animationTree.Set("parameters/Idle/blend_position", inputDir);
			animationTree.Set("parameters/Walk/blend_position", inputDir);
			initialPosition = Position;
			isMoving = true;
		}
	}

	private void Move(double delta)
	{
		ray.TargetPosition = inputDir * tileSize/2;
		ray.ForceRaycastUpdate();
		if (!ray.IsColliding())
		{
			percentMoved += speed * (float)delta;
			if (percentMoved >=1)
			{
				Position = initialPosition + inputDir * tileSize;
				isMoving = false;
				percentMoved = 0;
			}
			else
			{
				Position = initialPosition + inputDir * tileSize * percentMoved;
			}
		}
		else
		{
			isMoving = false;
		}
	}

	private void HandleAnimation()
	{
		if (inputDir == Vector2.Zero)
		{
			animationTree.Set("parameters/conditions/isWalking", false);
			animationTree.Set("parameters/conditions/Idle", true);
		}
		else
		{
			animationTree.Set("parameters/conditions/isWalking", true);
			animationTree.Set("parameters/conditions/Idle", false);
		}
	}
}
