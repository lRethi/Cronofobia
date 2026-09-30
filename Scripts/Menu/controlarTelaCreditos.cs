using Godot;
using System;

public partial class controlarTelaCreditos : Button
{
	[Export] public TextureRect telaCreditos;
	private AnimationPlayer animationPlayer;

	public override void _Ready()
	{
		this.Pressed += OnSelfPressed;
		animationPlayer = GetNode<AnimationPlayer>("%AnimationPlayer");
	}

	private async void OnSelfPressed()
	{
		if (telaCreditos.Visible)
		{
			animationPlayer.Play("creditosPop_out");
			await ToSignal(animationPlayer, AnimationPlayer.SignalName.AnimationFinished);
			telaCreditos.Visible = false;
		}
		else
		{
			telaCreditos.Visible = true;
			animationPlayer.Play("creditosPop_In");
		}
	}
}