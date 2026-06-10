using Godot;
using System;

public partial class Class : Resource
{
	[Export]
	public String className;
	[Export]
	public int baseAtk, baseDef, baseHp, baseMp, baseAtkPower;
	public readonly int[] expThresholds =
	{
		10, // 1 a 2
		30, // 2 a 3
		40, // 3 a 4
		50, // 4 a 5
		60, // 5 a 6
		70, // 6 a 7
		80, // 7 a 8
		90, // 8 a 9
		100 // 9 a 10
	};
}
