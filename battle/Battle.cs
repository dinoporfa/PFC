using Godot;
using System;
using System.Runtime.Serialization;

public partial class Battle : Control
{
	GameManager gm;
	Script gameInputs = ResourceLoader.Load<Script>("res://system/GameInputs.cs");
	BattleStarter battleStarter = new BattleStarter();
	Enemy enemy;
	Label textBox;
	int crit, ignoreDef, atkPower, player1Atk, player1Def, player1Hp, player2Atk, player2Def, player2Hp, enemyAtk, enemyDef, enemyHp;
	float roll;
	String player1Name, player2Name, enemyName;
	Party party = ResourceLoader.Load<Party>("res://system/party/Party.tres");

	[Signal]
	public delegate void APressedEventHandler();

	public override void _Ready()
	{
		gm = GetTree().Root.GetNode<GameManager>("GameManager");
		GetNode<Button>("VBoxContainer/Attack").GrabFocus();

		enemy = battleStarter.GetEnemy();

		GetNode<Sprite2D>("Player1Sprite").Texture = ((PlayerStats)party.partyMembers[0]).battleSprite;
		GetNode<Sprite2D>("Enemy1Sprite").Texture = enemy.sprite;

		enemyName = enemy.name;
		enemyAtk = enemy.atk;
		enemyDef = enemy.def;
		enemyHp = enemy.hp;
		player1Name = ((PlayerStats)party.partyMembers[0]).name;
		player1Atk = ((PlayerStats)party.partyMembers[0]).atk;
		player1Def = ((PlayerStats)party.partyMembers[0]).def;
		player1Hp = ((PlayerStats)party.partyMembers[0]).hp;

		textBox = GetNode<Label>("TextBackgound/TextBox");
		textBox.Text = enemyName + " appeared!";
	}

	private void OnAttackPressed()
	{
		PlayerTurn();
		EnemyTurn();
	}

	private void PlayerTurn()
	{
		int dmg;

		atkPower = 3;
		ignoreDef = 1;
		crit = 1;
		roll = 0.2f;

		dmg = (int)Math.Floor((player1Atk + atkPower - enemyDef * ignoreDef) * crit * (0.8 + roll));
		
		enemyHp -= dmg;

		textBox.Text = player1Name + " did " + dmg + " damage to " + enemyName + "!";
	}

	private void EnemyTurn()
	{
		int dmg;

		atkPower = 3;
		ignoreDef = 1;
		crit = 1;
		roll = 0.2f;

		dmg = (int)Math.Floor((enemyAtk + atkPower - player1Def * ignoreDef) * crit * (0.8 + roll));
		
		player1Hp -= dmg;

		textBox.Text = enemyName + " did " + dmg + " damage to " + player1Name + "!";
	}

	private void OnSpecialPressed()
	{
		textBox.Text = "Esto non está programado :(";
	}

	private async void OnEscapePressed()
	{
		Random random = new Random();
		int esc;
		esc = random.Next(0, 101);
		if (esc <= 80)
		{
			textBox.Text = "You escaped successfully!";
			await ToSignal(this, SignalName.APressed);
			gm.EndBattle();
		}
		else 
		{
			textBox.Text = "You could not escape!";
			await ToSignal(this, SignalName.APressed);
		}
		
	}
}
