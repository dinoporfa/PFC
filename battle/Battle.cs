using Godot;
using System;

public partial class Battle : Control
{
	GameManager gm;
	Label textBox;
	Sprite2D enemySprite;
	int playeAtk, playerDef, playerHealth, enemyAtk, enemyDef, enemyHealth;

	[Signal]
	public delegate void APressedEventHandler();

	public override void _Ready()
	{
		gm = GetTree().Root.GetNode<GameManager>("GameManager");
		textBox = GetNode<Label>("TextBackgound/TextBox");
		textBox.Text = "Enemy appeared!";
	}

    public override void _Process(double delta)
    {
        if (Input.IsActionJustPressed("a"))
		{
			EmitSignal(SignalName.APressed);
		}
    }

	private void OnAttackPressed()
	{
		
	}

	private void OnSpecialPressed()
	{
		
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
