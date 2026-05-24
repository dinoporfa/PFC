using Godot;
using System;
//por poñer para que se active o script só en menús
public partial class GameInputs : Node
{
	[Signal]
	public delegate void UpPressedEventHandler();
	[Signal]
	public delegate void DownPressedEventHandler();
	[Signal]
	public delegate void LeftPressedEventHandler();
	[Signal]
	public delegate void RightPressedEventHandler();
	[Signal]
	public delegate void APressedEventHandler();
	[Signal]
	public delegate void BPressedEventHandler();

    public override void _Input(InputEvent @event)
    {
		if (@event.IsActionPressed("up"))
			EmitSignal(SignalName.UpPressed);
		else if (@event.IsActionPressed("down"))
			EmitSignal(SignalName.DownPressed);
		else if (@event.IsActionPressed("left"))
			EmitSignal(SignalName.LeftPressed);
		else if (@event.IsActionPressed("right"))
			EmitSignal(SignalName.RightPressed);
		else if (@event.IsActionPressed("a"))
			EmitSignal(SignalName.APressed);
		else if (@event.IsActionPressed("b"))
			EmitSignal(SignalName.BPressed);
	}
	
}
