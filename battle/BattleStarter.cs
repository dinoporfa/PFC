using Godot;
using System;

public partial class BattleStarter : Node
{
	GameManager gm;
	Player player;
	int tilesMoved = 0, encounter = 0;
	Random random = new Random();

	public override void _Ready()
	{
		gm = GetTree().Root.GetNode<GameManager>("GameManager");
		player = GetTree().Root.GetNode<Player>(gm.currentScene.Name + "/Player");
		encounter = random.Next(10, 20);
	}

	public override void _Process(double delta)
	{
		tilesMoved = player.GetTilesMoved();
		if (tilesMoved >= encounter)
		{
			gm.StartBattle();
			player.ResetTilesMoved();
			encounter = random.Next(10, 20);
		}

	}

	//por adaptar para novas areas
	public Enemy GetEnemy(String zone)
	{
		Enemy enemy = new Enemy();
		int enemyNumber;
		String path = "res://zones/" + zone + "/enemies/";
		if (zone == "void")	
			enemyNumber = random.Next(0, 5);
		else
			enemyNumber = random.Next(0, 5);
		if (enemyNumber < 10)
			path += 0;
		path += enemyNumber + ".tres";
		enemy = ResourceLoader.Load<Enemy>(path);
		return enemy;
	}

	public String GetZone()
	{
		return "void";
	}
}
