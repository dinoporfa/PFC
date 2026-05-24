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
		GetTree().Root.RemoveChild(pastScene);
		pastScene.QueueFree();

		player = GetTree().Root.GetNode<Player>(currentScene.Name + "/Player");
		player.GlobalPosition = playerPosition;
		player.animationTree.Set("parameters/Idle/blend_position", new Vector2(0, 1));
	}

	public async void ChangeScene(String changeTo, Vector2 playerPosition, String direction)
	{
		player = GetTree().Root.GetNode<Player>(currentScene.Name + "/Player");
		player.CanMove(false);
		pastScene = currentScene;
		currentScene = ResourceLoader.Load<PackedScene>(changeTo).Instantiate();

		GetTree().Root.CallDeferred(Node.MethodName.AddChild, currentScene);
		await ToSignal(currentScene, Node.SignalName.Ready);
		GetTree().Root.RemoveChild(pastScene);
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
		player.CanMove(true);
	}

	public async void StartBattle(Enemy enemy)
	{
		Node battle = ResourceLoader.Load<PackedScene>("res://battle/Battle.tscn").Instantiate();
		GetTree().Root.CallDeferred(Node.MethodName.AddChild, battle);

		//await ToSignal(battle, Node.SignalName.Ready);

		GetTree().Root.RemoveChild(currentScene);
		//currentScene.QueueFree();
		
	}

	public async void EndBattle()
	{
		Node battle = ResourceLoader.Load<PackedScene>("res://battle/Battle.tscn").Instantiate();
		GetTree().Root.AddChild(currentScene);
		await ToSignal(currentScene, Node.SignalName.Ready);
		GetTree().Root.RemoveChild(battle);
	}
}
