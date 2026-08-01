using Godot;
using System;

public partial class HudMain : Control
{
	private Label textFome;
	private Label textSede;
	private Label textDinheiro;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		textFome = GetNode<Label>("objFome");
		textSede = GetNode<Label>("objSede");
		textDinheiro = GetNode<Label>("objDinheiro");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		textFome.Text = $"{NeedsState.Instance.varFome:0.00}/{NeedsState.Instance.maximoFome:0.00}";
		textSede.Text = $"{NeedsState.Instance.varSede:0.00}/{NeedsState.Instance.maximoSede:0.00}";
		textDinheiro.Text = $"R${NeedsState.Instance.varDinheiro:0.00}";
	}
}
