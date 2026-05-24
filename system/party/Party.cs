using Godot;
using System;

public partial class Party : Resource
{
	[Export]
	public Resource[] partyMembers = new Resource[3];
}
