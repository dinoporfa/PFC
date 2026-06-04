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
		30, // 2 a 3 (20exp)
		50, // 3 a 4 (20exp)
		70, // 4 a 5 (20exp)
		90, // 5 a 6 (20exp)
		110, // 6 a 7 (20exp)
		130, // 7 a 8 (20exp)
		150, // 8 a 9 (20exp)
		170, // 9 a 10 (20exp)
	};
}
