using Godot;
using System;

public partial class NeedsState : Node
{
    public static NeedsState Instance { get; private set; }

    public float varFome { get; private set; }
    public float varSede { get; private set; }
    public float varDinheiro { get; private set; }

    public float maximoFome { get; private set; } = 3f;
    public float maximoSede { get; private set; } = 3f;
    public float maximoDinheiro { get; private set; } = 100f;

    [Signal]
    public delegate void HungerChangedEventHandler(float newValue);

    [Signal]
    public delegate void ThirstChangedEventHandler(float newValue);

    [Signal]
    public delegate void MoneyChangedEventHandler(float newValue);

    public override void _Ready()
    {
        if (Instance != null && Instance != this)
        {
            QueueFree();
            return;
        }

        Instance = this;
    }

    public void SetFome(float value)
    {
        varFome = Mathf.Clamp(value, 0f, maximoFome);
		    GD.Print($"SetFome chamado: {varFome}");
        EmitSignal(SignalName.HungerChanged, varFome);
    }

    public void SetSede(float value)
    {
        varSede = Mathf.Clamp(value, 0f, maximoSede);
        EmitSignal(SignalName.ThirstChanged, varSede);
    }

    public void SetDinheiro(float value)
    {
        varDinheiro = Mathf.Clamp(value, 0f, maximoDinheiro);
        EmitSignal(SignalName.MoneyChanged, varDinheiro);
    }

    public void AlterarMaximoFome(float novoMaximo)
    {
        maximoFome = novoMaximo;
        SetFome(varFome);
    }

    public void AlterarMaximoSede(float novoMaximo)
    {
        maximoSede = novoMaximo;
        SetSede(varSede);
    }

    public void AlterarMaximoDinheiro(float novoMaximo)
    {
        maximoDinheiro = novoMaximo;
        SetDinheiro(varDinheiro);
    }

	public int GetMaximoFome()
	{
		return Mathf.RoundToInt(maximoFome);
	}
	public int GetMaximoSede()
	{
		return Mathf.RoundToInt(maximoSede);
	}
	public int GetMaximoDinheiro()
	{
		return Mathf.RoundToInt(maximoDinheiro);
	}
}