using Godot;
using System;
using DialogueManagerRuntime;
using Godot.Collections;

public partial class mecanicaIniciarDialogo : Node3D
{
	[Export] public Resource dialogue;
	[Export] public CharacterBody3D personagem;

	private bool dialogoAtivo = false;
	private bool playerPerto = false;

	[Export]
    public float alcanceInteracao = 1.25f;

	private bool bloqueandoInteracao = false;

	public override void _Ready()
	{
		var gs = GetNode("/root/GameState");
		DialogueManager.DialogueEnded += OnDialogueEnded;
	}

	public override void _ExitTree()
	{
		DialogueManager.DialogueEnded -= OnDialogueEnded;
	}

	public override void _Process(double delta)
	{
		if (personagem == null || dialogue == null || !Visible)
			return;

		float distancia = (personagem.GlobalPosition - GlobalPosition).Length();

		float alcance =
			EffectManager.Instance != null
				? EffectManager.Instance.GetInteractionRange(
					alcanceInteracao
				)
				: alcanceInteracao;

		playerPerto = distancia <= alcance;

		if (
			playerPerto &&
			!dialogoAtivo &&
			!bloqueandoInteracao &&
			Input.IsActionJustPressed("interact")
		)
		{
			IniciarDialogo();
		}
	}

		private void IniciarDialogo()
	{
		GD.Print("IniciarDialogo chamado");

		if (dialogoAtivo)
			return;

		dialogoAtivo = true;

		TimeState.Instance.CongelarTempo();

		var extraStates = new Array<Variant>
		{
			GetNode("/root/GameState")
		};

		DialogueManager.ShowDialogueBalloon(
			dialogue,
			"start",
			extraStates
		);
	}

	private async void OnDialogueEnded(Resource dialogueResource)
	{
		if (dialogueResource != dialogue)
			return;

		dialogoAtivo = false;

		TimeState.Instance.DescongelarTempo();

		bloqueandoInteracao = true;

		await ToSignal(
			GetTree().CreateTimer(0.15f),
			SceneTreeTimer.SignalName.Timeout
		);

		bloqueandoInteracao = false;
	}
}