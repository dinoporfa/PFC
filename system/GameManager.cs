using Godot;
using System;

public partial class GameManager : Node
{
	public Node currentScene, pastScene;
	Player player;
	public override void _Ready()
	{
		currentScene = GetTree().CurrentScene;
		
		Input.MouseMode = Input.MouseModeEnum.Hidden;
	}

	public async void StartPlay(String changeTo, Vector2 playerPosition)
	{
		pastScene = currentScene;
		currentScene = ResourceLoader.Load<PackedScene>(changeTo).Instantiate();

		GetTree().Root.CallDeferred(Node.MethodName.AddChild, currentScene);
		await ToSignal(currentScene, Node.SignalName.Ready);
		pastScene.QueueFree();

		player = GetTree().Root.GetNode<Player>(currentScene.Name + "/Player");
		player.GlobalPosition = playerPosition;
		player.animationTree.Set("parameters/Idle/blend_position", new Vector2(0, 1));
	}

	public async void ChangeMenuScene(String changeTo)
	{
		pastScene = currentScene;
		currentScene = ResourceLoader.Load<PackedScene>(changeTo).Instantiate();

		GetTree().Root.CallDeferred(Node.MethodName.AddChild, currentScene);
		await ToSignal(currentScene, Node.SignalName.Ready);
		pastScene.QueueFree();
	}

	public async void ChangeMapScene(String changeTo, Vector2 playerPosition, String direction)
	{
		player = GetTree().Root.GetNode<Player>(currentScene.Name + "/Player");
		player.CanMove(false);

		pastScene = currentScene;
		currentScene = ResourceLoader.Load<PackedScene>(changeTo).Instantiate();

		GetTree().Root.CallDeferred(Node.MethodName.AddChild, currentScene);
		await ToSignal(currentScene, Node.SignalName.Ready);
		pastScene.QueueFree();
		
		player = GetTree().Root.GetNode<Player>(currentScene.Name + "/Player");
		player.GlobalPosition = playerPosition;
		switch (direction)
		{
			case ("right"):
				player.animationTree.Set("parameters/Idle/blend_position", new Vector2(1, 0));
				break;
			case ("left"):
				player.animationTree.Set("parameters/Idle/blend_position", new Vector2(-1, 0));
				break;
			case ("up"):
				player.animationTree.Set("parameters/Idle/blend_position", new Vector2(0, -1));
				break;
			case ("down"):
				player.animationTree.Set("parameters/Idle/blend_position", new Vector2(0, 1));
				break;
		}
		player.ResetTilesMoved();
		player.CanMove(true);
	}

	public void StartBattle()
	{
		pastScene = currentScene;
		currentScene = ResourceLoader.Load<PackedScene>("res://battle/Battle.tscn").Instantiate();
		GetTree().Root.CallDeferred(Node.MethodName.AddChild, currentScene);

		GetTree().Root.RemoveChild(pastScene);
	}

	public void EndBattle()
	{
		GetTree().Root.CallDeferred(Node.MethodName.AddChild, pastScene);
		
		currentScene.QueueFree();

		currentScene = pastScene;
	}

    public void GameOver()
    {
		pastScene.QueueFree();
		pastScene = currentScene;
		
		currentScene = ResourceLoader.Load<PackedScene>("res://battle/GameOver.tscn").Instantiate();
		GetTree().Root.CallDeferred(Node.MethodName.AddChild, currentScene);
		
		pastScene.QueueFree();
    }

}