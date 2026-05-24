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
			gm.StartBattle(GetEnemy());
			player.ResetTilesMoved();
			encounter = random.Next(10, 20);
		}

	}

	//por adaptar para diferentes areas(por agora só devolve enemigo 0)
	public Enemy GetEnemy()
	{
		String path = "res://zones/void/enemies/";
		int enemyNumber = random.Next(0, 0);
		if (enemyNumber < 10)
			path += 0;
		path += enemyNumber + ".tres";
		Enemy enemy = ResourceLoader.Load<Enemy>(path);
		return enemy;
	}
}
