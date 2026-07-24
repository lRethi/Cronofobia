using Godot;
using System;

public partial class NeedsState : Node
{
	public static NeedsState Instance { get; private set; }

	public float varFome { get; private set;} // 0 -> 3
	public float varSede { get; private set;} // 0 -> 3
	public float varDinheiro { get; private set;} // 0 -> 100;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Instance = this;
	}

	public void SetFome(float value)
	{
		varFome = Mathf.Clamp(value, 0f, 3f);
	}

	public void SetSede(float value)
	{
		varSede = Mathf.Clamp(value, 0f, 3f);
	}

	public void SetDinheiro(float value)
	{
		varDinheiro = Mathf.Clamp(value, 0f, 100f);
	}
}
