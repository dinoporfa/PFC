using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Threading.Tasks;

public partial class Battle : Control
{
	GameManager gm;
	Party party = ResourceLoader.Load<Party>("res://system/party/Party.tres");
	BattleStarter battleStarter = new BattleStarter();
	Label textBox, menuTextBox, player1HpLabel, player2HpLabel;
	ColorRect textBoxBackground;
	Button atkButton, spcButton, escButton;
	ProgressBar player1HpBar, player2HpBar;
	AnimationPlayer animationPlayer;
	int crit, ignoreDef, turn, enemy1Hp, enemy2Hp, enemy3Hp;
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
		
		atkButton = GetNode<Button>("VBoxContainer/Attack");
		spcButton = GetNode<Button>("VBoxContainer/Special");
		escButton = GetNode<Button>("VBoxContainer/Escape");
		player1HpBar = GetNode<ProgressBar>("Player1HpBar");
		player1HpLabel = GetNode<Label>("Player1HpBar/Player1Hp");
		player2HpBar = GetNode<ProgressBar>("Player2HpBar");
		player2HpLabel = GetNode<Label>("Player2HpBar/Player2Hp");
		menuTextBox = textBox = GetNode<Label>("MenuTextBackgound/MenuTextBox");
		textBox = GetNode<Label>("TextBackground/TextBox");
		textBoxBackground = GetNode<ColorRect>("TextBackground");
		animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");

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

		turn = 0;

		menuTextBox.Text = "What should " + players[0].name + " do?";
		await ToSignal(this, SignalName.APressed);
		textBox.Hide();
		textBoxBackground.Hide();
		atkButton.GrabFocus();
    }

    public override void _Input(InputEvent @event)
    {
		if (!animationPlayer.IsPlaying())
		{
			if (Input.IsActionJustPressed("a"))
			{
			if (textBox.Visible)
				EmitSignal(SignalName.APressed);
			else
				Call("On" + GetViewport().GuiGetFocusOwner().Name + "Pressed");
			}
		}
    }

	private async void Play()
	{
		//meter as accións enemigas
		int enemyId, playerAttacked;
		for (int i = 0; i < enemies.Count; i++)
		{
			enemyId = i;
			do
			{
				playerAttacked = random.Next(0, players.Count());
			}while(players[playerAttacked] == null);
			
			queue.Add( () => EnemyAttack(enemyId, playerAttacked));
		}
		
		//executar queue
		for (int i = 0; i < queue.Count; i++)
		{
			await queue[i]();
		}
		queue.Clear();
	}

	private void OnAttackPressed()
	{
		int playerId = turn;
		queue.Add( () => PlayerAttack(playerId, 0));
		turn++;
		if (turn == players.Count)
		{
			Play();
			turn = 0;
		}
		menuTextBox.Text = "What should " + players[turn].name + " do?";
	}

	private async Task PlayerAttack(int playerId, int enemyId)
	{
		textBox.Show();
		textBoxBackground.Show();
		int dmg;

		animationPlayer.Play("enemy1Hit");

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

		dmg = (int)Math.Floor((players[playerId].atk + players[playerId].charClass.baseAtkPower - enemies[enemyId].def * ignoreDef) * crit * (0.8 + roll));
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
		
		textBox.Hide();
		textBoxBackground.Hide();
	}

	private async Task EnemyAttack(int enemyId, int playerId)
	{
		textBox.Show();
		textBoxBackground.Show();

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
			players[playerId].currentHp = 0;

		switch (playerId)
		{
			case 0:
				player1HpBar.Value = players[playerId].currentHp;
				player1HpLabel.Text = players[playerId].currentHp + "/" + players[playerId].maxHp;
				((Character)party.partyMembers[playerId]).currentHp = players[playerId].currentHp;
				break;
			case 1:
				player2HpBar.Value = players[playerId].currentHp;
				player2HpLabel.Text = players[playerId].currentHp + "/" + players[playerId].maxHp;
				((Character)party.partyMembers[playerId]).currentHp = players[playerId].currentHp;
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
			gm.GameOver();
		}
			
		textBox.Hide();
		textBoxBackground.Hide();
	}

	private async void OnSpecialPressed()
	{
		menuTextBox.Text = "Esto non está programado :(";
		await Task.Delay(500);
		menuTextBox.Text = "What should " + players[turn].name + " do?";
	}

	private async void OnCheckPressed()
	{
		menuTextBox.Text = "Esto tampouco está programado :(";
		await Task.Delay(500);
		menuTextBox.Text = "What should " + players[turn].name + " do?";
	}

	private async void OnEscapePressed()
	{
		GetViewport().GuiReleaseFocus();
		textBox.Show();
		textBoxBackground.Show();
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
			Play();
			atkButton.GrabFocus();
		}
		
	}

	private async Task PlayerDefeated(int playerId)
	{
		animationPlayer.Play("player" + (playerId+1) + "Defeated");

		textBox.Text = players[playerId].name + " was defeted!";
		await ToSignal(this, SignalName.APressed);

		players[playerId] = null;

		if (players[0] == null && players[1] == null)
		{
			textBox.Text = "You lost!";
			await ToSignal(this, SignalName.APressed);
			gm.GameOver();
		}
	}

	private async Task EnemyDefeated(int enemyId)
	{
		animationPlayer.Play("enemy" + (enemyId+1) + "Defeated");

		textBox.Text = enemies[enemyId].name + " was defeted!";
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
		((Character)party.partyMembers[charId]).lvl += 1;
			
		textBox.Text = ((Character)party.partyMembers[charId]).name + " leveled up to level " + ((Character)party.partyMembers[0]).lvl + "!";
		
		
		await ToSignal(this, SignalName.APressed);
	}
}
