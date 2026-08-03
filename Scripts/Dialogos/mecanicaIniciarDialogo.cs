using Godot;
using System;
using DialogueManagerRuntime;
using Godot.Collections;

public partial class mecanicaIniciarDialogo : StaticBody3D
{
	[Export] public Resource dialogue;
	[Export] public CharacterBody3D personagem;

	private bool dialogoAtivo = false;
	private bool playerPerto = false;

	public override void _Ready()
	{
		var gs = GetNode("/root/GameState");
GD.Print(gs);
		DialogueManager.DialogueEnded += OnDialogueEnded;
	}

	public override void _ExitTree()
	{
		DialogueManager.DialogueEnded -= OnDialogueEnded;
	}

	public override void _Process(double delta)
	{
		if (personagem == null || dialogue == null)
			return;

		float distancia = (personagem.GlobalPosition - GlobalPosition).Length();
		playerPerto = distancia <= 1.25f;

		if (playerPerto && Input.IsActionJustPressed("interact"))
		{
			IniciarDialogo();
		}
	}

	private void IniciarDialogo()
	{
		TimeState.Instance.CongelarTempo();
		GD.Print("IniciarDialogo chamado");

		if (dialogoAtivo)
			return;

		dialogoAtivo = true;

		var extraStates = new Array<Variant>
		{
			GetNode("/root/GameState")
		};
		DialogueManager.ShowDialogueBalloon(dialogue, "start", extraStates);
	}

	private void OnDialogueEnded(Resource dialogueResource)
	{
		TimeState.Instance.DescongelarTempo();
		if (dialogueResource != dialogue)
			return;

		dialogoAtivo = false;
	}
}