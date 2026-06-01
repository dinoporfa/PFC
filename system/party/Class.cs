using Godot;
using System;

public partial class Class : Resource
{
	[Export]
	public String className;
	[Export]
	public int baseAtk, baseDef, baseHp, baseAtkPower;
	public readonly int[] expThresholds =
	{
		10, // 1 a 2
		30, // 2 a 3
	};
}
