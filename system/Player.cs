using Godot;
using System;
using System.Runtime.CompilerServices;

public partial class Player : CharacterBody2D
{
	GameManager gm;
	const int tileSize = 16;
	static int tilesMoved = 0;
	float playerSpeed = 0.3f;
	Vector2 initialPosition;
	static Vector2 inputDir;
	bool isMoving = false, canMove = true;
	public AnimationTree animationTree;
	static RayCast2D ray;

	public void CanMove(bool canMove)
	{
		this.canMove = canMove;
	}

	public void ResetTilesMoved()
	{
		tilesMoved = 0;
	}

	public int GetTilesMoved()
	{
		return tilesMoved;
	}

	public static Vector2 GetInputDir()
	{
		return inputDir;
	}

	public override void _Ready()
	{
		gm = GetTree().Root.GetNode<GameManager>("GameManager");

		this.SetScript(ResourceLoader.Load<Script>("res://system/Player.cs"));

		initialPosition = GlobalPosition;
		animationTree = GetTree().Root.GetNode<AnimationTree>(gm.currentScene.Name + "/Player/AnimationTree");
		animationTree.Active = true;
		ray = GetTree().Root.GetNode<RayCast2D>(gm.currentScene.Name + "/Player/RayCast2D");
	}

    public override void _PhysicsProcess(double delta)
	{
		if (canMove)
		{
			HandleAnimation();
			if (!isMoving)
				ProcessInput();
			else if (inputDir == Vector2.Zero)
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
			initialPosition = GlobalPosition;
			isMoving = true;
			Move();
		}
	}

	private async void Move()
	{
		ray.TargetPosition = inputDir * tileSize/2;
		ray.ForceRaycastUpdate();
		if (!ray.IsColliding())
		{
			Tween tween = GetTree().CreateTween();
			tween.TweenProperty(this, "position", (initialPosition + inputDir * tileSize), playerSpeed);
			await ToSignal(tween, "finished");
			tilesMoved += 1;
		}
		isMoving = false;
	}

	private void HandleAnimation()
	{
		if (inputDir == Vector2.Zero)
		{
			animationTree.Set("parameters/conditions/isWalking", false);
			animationTree.Set("parameters/conditions/stopWalking", true);
		}
		else
		{
			animationTree.Set("parameters/conditions/isWalking", true);
			animationTree.Set("parameters/conditions/stopWalking", false);
		}
	}
}
