using Godot;
using System;

public partial class Character : Resource
{
	[Export]
	public String name;
	[Export]
	public Class charClass;
	[Export]
	public int charId, atk, def, maxHp, currentHp, lvl, exp;
	[Export]
	public Texture2D sprite, battleSprite, face;
}
