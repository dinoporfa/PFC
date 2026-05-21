using Godot;
using System;

public partial class Enemy : Resource
{
	[Export]
	String name;
	[Export]
	Texture sprite;
	[Export]
	int hp, atk, def;
}
