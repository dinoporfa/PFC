using Godot;
using System;

public partial class GameOver : Control
{
	GameManager gm;
	Party party = ResourceLoader.Load<Party>("res://system/party/Party.tres");
    public override void _Ready()
    {
		gm = GetTree().Root.GetNode<GameManager>("GameManager");
        GetNode<Button>("CenterContainer/VBoxContainer/Retry").GrabFocus();
    }

	public override void _Input(InputEvent @event)
    {
        if (Input.IsActionJustPressed("a"))
			Call("On" + GetViewport().GuiGetFocusOwner().Name + "Pressed");
    }

	private void OnRetryPressed()
	{
		for (int i = 0; i < party.partyMembers.Length; i++)
		{
			((Character)party.partyMembers[i]).currentHp = ((Character)party.partyMembers[i]).maxHp;
			((Character)party.partyMembers[i]).exp = 0;
			((Character)party.partyMembers[i]).lvl = 1;
			((Character)party.partyMembers[i]).isDead = false;
		}
		gm.StartPlay("res://zones/void/Void.tscn", new Vector2(13*16, 12*16));
	}

	private void OnQuitPressed()
	{
		gm.ChangeMenuScene("res://system/MainMenu.tscn");
	}
}
