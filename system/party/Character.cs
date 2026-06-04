using Godot;
using System;

public partial class Character : Resource
{
	[Export]
	public String name;
	[Export]
	public Class charClass;
	[Export]
	public int currentHp, lvl, exp;
	public int atk, def, maxHp, maxMp, atkPower;
	[Export]
	public bool isDead;
	[Export]
	public Texture2D sprite, battleSprite, face;

	public void CalcStats()
	{
		maxHp = (int)Math.Floor(2 * charClass.baseAtk * 0.1 * lvl);
		if(maxHp < 10)
		{
			maxHp = 10;
		}

		maxMp = (int)Math.Floor(2 * charClass.baseMp * 0.1 * lvl);
		if(maxMp < 10)
		{
			maxMp = 10;
		}

		atk = (int)Math.Floor(2 * charClass.baseAtk * 0.1 * lvl);
		if(atk == 0)
		{
			atk = 1;
		}

		def = (int)Math.Floor(2 * charClass.baseDef * 0.1 * lvl);
		if(def == 0)
		{
			def = 1;
		}

		atkPower = charClass.baseAtkPower;
	}
}
