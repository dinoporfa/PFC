using Godot;
using System;

public partial class PlayerStats : Resource
{
	[Export]
	public String name;
	[Export]
	public Texture2D sprite, battleSprite, face;
	[Export]
	public int hp, atk, def, lvl;
}
