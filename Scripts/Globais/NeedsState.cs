using Godot;

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
        varFome = Mathf.Clamp(
            value,
            0f,
            maximoFome
        );

        EmitSignal(
            SignalName.HungerChanged,
            varFome
        );
    }

    public void SetSede(float value)
    {
        varSede = Mathf.Clamp(
            value,
            0f,
            maximoSede
        );

        EmitSignal(
            SignalName.ThirstChanged,
            varSede
        );
    }

    public void SetDinheiro(float value)
    {
        varDinheiro = Mathf.Clamp(
            value,
            0f,
            maximoDinheiro
        );

        EmitSignal(
            SignalName.MoneyChanged,
            varDinheiro
        );
    }

    public void AumentarDinheiro(float value)
    {
        GD.Print("[NeedsState] AumentarDinheiro chamado. Antes: ", varDinheiro);

        SetDinheiro(varDinheiro + value);

        GD.Print("[NeedsState] AumentarDinheiro concluído. Depois: ", varDinheiro);

        EmitSignal(
            SignalName.MoneyChanged,
            varDinheiro
        );
    }

    public void AlterarMaximoFome(float novoMaximo)
    {
        maximoFome = Mathf.Max(
            1f,
            novoMaximo
        );

        SetFome(varFome);
    }

    public void AlterarMaximoSede(float novoMaximo)
    {
        maximoSede = Mathf.Max(
            1f,
            novoMaximo
        );

        SetSede(varSede);
    }

    public void AlterarMaximoDinheiro(float novoMaximo)
    {
        maximoDinheiro = Mathf.Max(
            0f,
            novoMaximo
        );

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

    public void Resetar()
    {
        varFome = 0f;
        varSede = 0f;
        varDinheiro = 0f;

        maximoFome = 3f;
        maximoSede = 3f;
        maximoDinheiro = 100f;

        EmitSignal(SignalName.HungerChanged, varFome);
        EmitSignal(SignalName.ThirstChanged, varSede);
        EmitSignal(SignalName.MoneyChanged, varDinheiro);
    }
}