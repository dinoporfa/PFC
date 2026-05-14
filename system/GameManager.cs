using Godot;
using System;

public partial class GameManager : Node
{
	Node currentScene, pastScene;
	public override void _Ready()
	{
		currentScene = GetTree().CurrentScene;
	}

	public void ChangeScene(String changeTo)
	{
		pastScene = currentScene;
		currentScene = ResourceLoader.Load<PackedScene>(changeTo).Instantiate();;
		GetTree().Root.CallDeferred(Node.MethodName.AddChild, currentScene);
		GetTree().Root.CallDeferred(Node.MethodName.RemoveChild, pastScene);
		pastScene.QueueFree();
	}
}
