using Godot;
using System;

public partial class testShitassDialogue : Node3D
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		var dm = GetNode("/root/DialogueManager");
		var dialogue = GD.Load("res://Assets/Dialogos/teste.dialogue");

		dm.Call("show_dialogue_balloon", dialogue, "start");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
