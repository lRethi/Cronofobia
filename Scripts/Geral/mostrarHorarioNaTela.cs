using Godot;
using System;

public partial class mostrarHorarioNaTela : Control
{
	private Label textStuff;

	private TimeState timeState;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		GD.Print(TimeState.Instance);
		timeState = TimeState.Instance;
		textStuff = GetNode<Label>("Label");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		textStuff.Text = timeState.horarioFormatado;
	}
}
