using Godot;
using System;

public partial class PlayerStats : Resource
{
	[Export]
	String name;
	[Export]
	Texture sprite, battleSprite, face;
	[Export]
	int hp, atk, def, lvl;
}
