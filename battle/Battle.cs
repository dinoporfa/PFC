using Godot;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters;
using System.Threading.Tasks;

public partial class Battle : Control
{
	GameManager gm;
	Party party = ResourceLoader.Load<Party>("res://system/party/Party.tres");
	BattleStarter battleStarter = new BattleStarter();
	Label textBox, player1HpLabel, player2HpLabel;
	VBoxContainer actionMenu;
	Button atkButton, spcButton, escButton;
	ProgressBar player1HpBar, player2HpBar;
	AnimationPlayer animationPlayer;
	int crit, ignoreDef, playerTurn, enemy1Hp, enemy2Hp, enemy3Hp;
	float roll;
	Random random = new Random();
	List<Func<Task>> queue = new List<Func<Task>>();
	List<Character> players = new List<Character>();
	List<Enemy> enemies = new List<Enemy>();

	[Signal]
	public delegate void APressedEventHandler();
	
	public override async void _Ready()
	{
		GetViewport().GuiReleaseFocus();
		gm = GetTree().Root.GetNode<GameManager>("GameManager");
		
		actionMenu = GetNode<VBoxContainer>("ActionMenu");
		atkButton = GetNode<Button>("ActionMenu/Attack");
		spcButton = GetNode<Button>("ActionMenu/Special");
		escButton = GetNode<Button>("ActionMenu/Escape");
		player1HpBar = GetNode<ProgressBar>("Player1HpBar");
		player1HpLabel = GetNode<Label>("Player1HpBar/Player1Hp");
		player2HpBar = GetNode<ProgressBar>("Player2HpBar");
		player2HpLabel = GetNode<Label>("Player2HpBar/Player2Hp");
		textBox = GetNode<Label>("TextBackground/TextBox");
		animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");

		actionMenu.Hide();

		//list de xogadores
		for (int i = 0; i < party.partyMembers.Length; i++)
		{
			players.Add((Character)party.partyMembers[i]);
			GetNode<Sprite2D>("Player" + (i+1) + "Sprite").Texture = ((Character)party.partyMembers[i]).battleSprite;
		}

		//list  de enemigos(1, 2 ou 3)(por agora só un)
		if (random.Next(0,1) == 1)
		{
			if (random.Next(0,2) == 1)
			{
				for (int i = 0; i < 2; i++)
				{
					enemies.Add(battleStarter.GetEnemy(battleStarter.GetZone()));
					GetNode<Sprite2D>("Enemy" + (i+1) + "Sprite").Texture = enemies[i].sprite;
				}
			}
			else
			{
				for (int i = 0; i < 3; i++)
				{
					enemies.Add(battleStarter.GetEnemy(battleStarter.GetZone()));
					GetNode<Sprite2D>("Enemy" + (i+1) + "Sprite").Texture = enemies[i].sprite;
				}
			}
			textBox.Text = "A group of enemies appeared!";
		}
		else
		{
			enemies.Add(battleStarter.GetEnemy(battleStarter.GetZone()));
			GetNode<Sprite2D>("Enemy1Sprite").Texture = enemies[0].sprite;
			textBox.Text = enemies[0].name + " appeared!";
			enemy1Hp = enemies[0].hp;
		}

		player1HpBar.MaxValue = players[0].maxHp;
		player1HpBar.Value = players[0].currentHp;
		player1HpLabel.Text = players[0].currentHp + "/" + players[0].maxHp;
		player2HpBar.MaxValue = players[1].maxHp;
		player2HpBar.Value = players[1].currentHp;
		player2HpLabel.Text = players[1].currentHp + "/" + players[0].maxHp;

		playerTurn = 0;
		while(players[playerTurn].currentHp == 0)
		{
			playerTurn++;
		}

		await ToSignal(this, SignalName.APressed);
		textBox.Text = "What should " + players[0].name + " do?";textBox.Text = "What should " + players[0].name + " do?";
		actionMenu.Show();
		atkButton.GrabFocus();
    }

    public override void _Process(double delta)
    {
        player1HpLabel.Text = Math.Floor(player1HpBar.Value) + "/" + players[0].maxHp;
		player2HpLabel.Text = Math.Floor(player2HpBar.Value) + "/" + players[1].maxHp;
	}

    public override void _Input(InputEvent @event)
    {
		if (!animationPlayer.IsPlaying())
		{
			if (Input.IsActionJustPressed("a"))
			{
			if (!actionMenu.Visible)
				EmitSignal(SignalName.APressed);
			else
				Call("On" + GetViewport().GuiGetFocusOwner().Name + "Pressed");
			}
			else
			{
				if(Input.IsActionJustPressed("b"))
				{
					if (!textBox.Visible)
					{
						if (queue.Count > 0)
						{
							queue.RemoveAt(queue.Count - 1);
							playerTurn--;
							textBox.Text = "What should " + players[playerTurn].name + " do?";
						}
					}
				}
			}
		}
    }

	private async Task Play()
	{
		//meter as accións enimigas
		int enemyId, playerAttacked;
		for (int i = 0; i < enemies.Count; i++)
		{
			enemyId = i;
			playerAttacked = GetAlivePlayers()[random.Next(0, GetAlivePlayers().Count)];
			queue.Add( () => EnemyAttack(enemyId, playerAttacked));
		}
		
		//executar queue
		for (int i = 0; i < queue.Count; i++)
		{
			await queue[i]();
		}
		queue.Clear();
	}

	private async void OnAttackPressed()
	{
		while(players[playerTurn].currentHp == 0)
		{
			playerTurn++;
		}

		int playerId = playerTurn;
		queue.Add( () => PlayerAttack(playerId, 0));

		playerTurn++;
		if (playerTurn >= GetAlivePlayers().Count)
		{
			await Play();

			playerTurn = 0;
			if (GetAlivePlayers().Count > 0)
				while(players[playerTurn].currentHp == 0)
				{
					playerTurn++;
				}
		}
		textBox.Text = "What should " + players[playerTurn].name + " do?";
		atkButton.GrabFocus();
	}

	private async Task PlayerAttack(int playerId, int enemyId)
	{
		actionMenu.Hide();
		GetViewport().GuiReleaseFocus();
		int dmg;

		animationPlayer.Play("enemy" + (enemyId+1) + "Hit");

		textBox.Text = players[playerId].name + " attacks!";
		await ToSignal(this, SignalName.APressed);

		if (random.Next(0,101) <= 4)
		{
			crit = 2;
		}
		else
		{
			crit = 1;
		}
		roll = random.NextSingle() * 0.2f;
		ignoreDef = 1;

		dmg = (int)Math.Floor((players[playerId].atk + players[playerId].atkPower - enemies[enemyId].def * ignoreDef) * crit * (0.8 + roll));
		if (dmg <= 0)
			dmg = 1;
		enemy1Hp -= dmg;

		if (crit == 2)
		{
			textBox.Text = "A critical hit!";
			await ToSignal(this, SignalName.APressed);
		}

		textBox.Text = players[playerId].name + " did " + dmg + " damage to " + enemies[enemyId].name + "!";
		await ToSignal(this, SignalName.APressed);

		if (enemy1Hp <= 0)
		{
			await EnemyDefeated(enemyId);
			gm.EndBattle();
		}
		
		actionMenu.Show();
	}

	private async Task EnemyAttack(int enemyId, int playerId)
	{
		actionMenu.Hide();

		animationPlayer.Play("player" + (playerId + 1) + "Hit");

		textBox.Text = enemies[enemyId].name + " attacks!";
		await ToSignal(this, SignalName.APressed);

		int dmg;
		
		if (random.Next(0,101) <= 4)
		{
			crit = 2;
		}
		else
		{
			crit = 1;
		}
		roll = random.NextSingle() * 0.2f;
		ignoreDef = 1;


		dmg = (int)Math.Floor((enemies[enemyId].atk + enemies[enemyId].atkPower - players[playerId].def * ignoreDef) * crit * (0.8 + roll));
		if (dmg <= 0)
			dmg = 1;
		players[playerId].currentHp -= dmg;


		if (players[playerId].currentHp < 0)
		{
			players[playerId].currentHp = 0;
		}

		((Character)party.partyMembers[playerId]).currentHp = players[playerId].currentHp;

		switch (playerId)
		{
			case 0:
				Tween bar1 = GetTree().CreateTween();
				bar1.TweenProperty(player1HpBar, "value", players[playerId].currentHp, 0.1);
				await ToSignal(bar1, "finished");
				break;
			case 1:
				Tween bar2 = GetTree().CreateTween();
				bar2.TweenProperty(player2HpBar, "value", players[playerId].currentHp, 0.1);
				await ToSignal(bar2, "finished");
				break;
		}

		if (crit == 2)
		{
			textBox.Text = "A critical hit!";
			await ToSignal(this, SignalName.APressed);
		}

		textBox.Text = enemies[enemyId].name + " did " + dmg + " damage to " + players[playerId].name + "!";
		await ToSignal(this, SignalName.APressed);

		if (players[playerId].currentHp <= 0)
		{
			await PlayerDefeated(playerId);
		}
			
		actionMenu.Show();
		atkButton.GrabFocus();
	}

	private async void OnSpecialPressed()
	{
		textBox.Text = "Esto non está programado :(";
		await Task.Delay(500);
		textBox.Text = "What should " + players[playerTurn].name + " do?";
	}

	private async void OnCheckPressed()
	{
		while(players[playerTurn].currentHp == 0)
		{
			playerTurn++;
		}

		queue.Add( () =>EnemyCheck(0));

		playerTurn++;
		if (playerTurn >= GetAlivePlayers().Count)
		{
			await Play();

			playerTurn = 0;
			if (GetAlivePlayers().Count > 0)
				while(players[playerTurn].currentHp == 0)
				{
					playerTurn++;
				}
		}
		textBox.Text = "What should " + players[playerTurn].name + " do?";
		atkButton.GrabFocus();
	}

	private async Task EnemyCheck(int enemyId)
	{
		GetViewport().GuiReleaseFocus();
		actionMenu.Hide();

		textBox.Text = enemies[enemyId].name + "\nAttack: " + enemies[enemyId].atk + "\nDefense: " + enemies[enemyId].def;
		await ToSignal(this, SignalName.APressed);
	}

	private async void OnEscapePressed()
	{
		GetViewport().GuiReleaseFocus();
		actionMenu.Hide();
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
			await Play();
			atkButton.GrabFocus();
		}
		
	}

	private List<int> GetAlivePlayers()
	{
		List<int> alivePlayers = new List<int>();
		for (int i = 0; i < players.Count; i++)
			if (players[i].currentHp > 0)
				alivePlayers.Add(i);

		return alivePlayers;
	}

	private async Task PlayerDefeated(int playerId)
	{
		animationPlayer.Play("player" + (playerId+1) + "Defeated");

		textBox.Text = players[playerId].name + " was defeated!";
		await ToSignal(this, SignalName.APressed);

		if (players[0].currentHp == 0 && players[1].currentHp == 0)
		{
			textBox.Text = "...";
			await ToSignal(this, SignalName.APressed);
			textBox.Text = "You lost!";
			await ToSignal(this, SignalName.APressed);
			gm.GameOver();
		}
	}

	private async Task EnemyDefeated(int enemyId)
	{
		animationPlayer.Play("enemy" + (enemyId+1) + "Defeated");

		textBox.Text = enemies[enemyId].name + " was defeated!";
		await ToSignal(this, SignalName.APressed);
		
		await ExpGain(enemyId);
	}

	private async Task ExpGain(int enemyId)
	{
		int expGained = (int)Math.Floor((double)(enemies[enemyId].exp / players.Count));
		for (int i = 0; i < players.Count; i++)
		{
			if (((Character)party.partyMembers[i]).lvl < 3)
			{
				textBox.Text = ((Character)party.partyMembers[i]).name + " gained " + expGained + " EXP!";
				((Character)party.partyMembers[i]).exp += expGained;
				await ToSignal(this, SignalName.APressed);
		
				while(((Character)party.partyMembers[i]).exp >= ((Character)party.partyMembers[i]).charClass.expThresholds[((Character)party.partyMembers[i]).lvl-1])
				{
					await LvlUp(i);
					if (((Character)party.partyMembers[i]).lvl >= 3)
						break;
				}
			}	
		}
		
	}

	private async Task LvlUp(int charId)
	{
		ColorRect lvlUpBackground;
		Label lvl, hp, mp, atk, def, ability;
		int lastHp, lastMp, lastAtk, lastDef;
		lvlUpBackground = GetNode<ColorRect>("LvlUpMenu");
		lvl = GetNode<Label>("LvlUpMenu/VBoxContainer/LvlUp");
		hp = GetNode<Label>("LvlUpMenu/VBoxContainer/HpUp");
		mp = GetNode<Label>("LvlUpMenu/VBoxContainer/MpUp");
		atk = GetNode<Label>("LvlUpMenu/VBoxContainer/AtkUp");
		def = GetNode<Label>("LvlUpMenu/VBoxContainer/DefUp");
		ability = GetNode<Label>("LvlUpMenu/VBoxContainer/NewAbility");
		lastHp = ((Character)party.partyMembers[charId]).maxHp;
		lastMp = ((Character)party.partyMembers[charId]).maxMp;
		lastAtk = ((Character)party.partyMembers[charId]).atk;
		lastDef = ((Character)party.partyMembers[charId]).def;


		textBox.Text = ((Character)party.partyMembers[charId]).name + " leveled up!";
		lvlUpBackground.Show();

		lvl.Text = "Level Up!\n" + ((Character)party.partyMembers[charId]).lvl + " -> " + (((Character)party.partyMembers[charId]).lvl + 1);
		hp.Text = "Hp = " + lastHp;
		mp.Text = "Mp = " + lastMp;
		atk.Text = "Atk = " + lastAtk;
		def.Text = "Def = " + lastDef;

		((Character)party.partyMembers[charId]).lvl += 1;
		((Character)party.partyMembers[charId]).CalcStats();

		if (((Character)party.partyMembers[charId]).maxHp - lastHp != 0)
		{
			hp.Text += " + " + (((Character)party.partyMembers[charId]).maxHp - lastHp);
		}
		if (((Character)party.partyMembers[charId]).maxMp - lastMp != 0)
		{
			mp.Text += " + " + (((Character)party.partyMembers[charId]).maxMp - lastMp);
		}
		if (((Character)party.partyMembers[charId]).atk - lastAtk != 0)
		{
			atk.Text += " + " + (((Character)party.partyMembers[charId]).atk - lastAtk);
		}
		if (((Character)party.partyMembers[charId]).def - lastDef != 0)
		{
			def.Text += " + " + (((Character)party.partyMembers[charId]).def - lastDef);
		}
		ability.Text = "";
		await ToSignal(this, SignalName.APressed);

		hp.Text = "Hp = " + ((Character)party.partyMembers[charId]).maxHp;
		mp.Text = "Mp = " + ((Character)party.partyMembers[charId]).maxMp;
		atk.Text = "Atk = " + ((Character)party.partyMembers[charId]).atk;
		def.Text = "Def = " + ((Character)party.partyMembers[charId]).def;
		await ToSignal(this, SignalName.APressed);

		lvlUpBackground.Hide();
	}
}