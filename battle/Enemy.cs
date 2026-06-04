using Godot;
using System;

public partial class Enemy : Resource
{
	[Export]
	public String name;
	[Export]
	public Texture2D sprite;
	[Export]
	public int hp, atk, atkPower, def, exp;

}
