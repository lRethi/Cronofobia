using Godot;
using System;

public partial class NeedsState : Node
{
	public static NeedsState Instance { get; private set; }

	public float varFome { get; private set;} // 0 -> 3
	public float varSede { get; private set;} // 0 -> 3
	public float varDinheiro { get; private set;} // 0 -> 100;

	public float maximoFome { get; private set;} = 3f;
	public float maximoSede { get; private set;} = 3f;
	public float maximoDinheiro { get; private set;} = 100f;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
		{
			if (Instance != null && Instance != this) {
			QueueFree();
			return;
		}
		Instance = this;
		Instance = this;
	}

	public void SetFome(float value)
	{
		varFome = Mathf.Clamp(value, 0f, maximoFome);
	}

	public void SetSede(float value)
	{
		varSede = Mathf.Clamp(value, 0f, maximoSede);
	}

	public void SetDinheiro(float value)
	{
		varDinheiro = Mathf.Clamp(value, 0f, maximoDinheiro);
	}

	public void AlterarMaximoFome(float novoMaximo)
	{
		maximoFome = novoMaximo;
		varFome = Mathf.Clamp(varFome, 0f, maximoFome);
	}
	public void AlterarMaximoSede(float novoMaximo)
	{
		maximoSede = novoMaximo;
		varSede = Mathf.Clamp(varSede, 0f, maximoSede);
	}
	public void AlterarMaximoDinheiro(float novoMaximo)
	{
		maximoDinheiro = novoMaximo;
		varDinheiro = Mathf.Clamp(varDinheiro, 0f, maximoDinheiro);
	}
}
