using Godot;
using System;

public partial class TimeState : Node
{
    public static TimeState Instance { get; private set; }

    [Export]
    public float duracaoDiaSegundos = 120f;

    public float minutoDoDia { get; private set; } = 480f;
    public float minutoInicioDia { get; private set; } = 480f;
    public float minutoFimNoite { get; private set; } = 1320f;

    public float horaDecimal =>
        minutoDoDia / 60f;

    public float tempoNormalizado =>
        minutoDoDia / 1440f;

    public int tempoHoras =>
        (int)(minutoDoDia / 60f);

    public int tempoMinutos =>
        (int)(minutoDoDia % 60f);

    public string horarioFormatado =>
        $"{tempoHoras:D2}:{tempoMinutos:D2}";

    public float escalaTempo = 1f;

    public int diaAtual { get; private set; } = 1;
    public int maximoDias { get; private set; } = 7;

    [Export]
    public int maximoDiasSemComer { get; private set; } = 2;

    [Export]
    public int maximoDiasSemBeber { get; private set; } = 2;

    [Export]
    public int maximoDiasSemAbrigo { get; private set; } = 2;

    public int diasSemComer { get; private set; }
    public int diasSemBeber { get; private set; }
    public int diasSemAbrigo { get; private set; }

    public static bool lugarParaDormir = true;
    public static bool comidaSuficiente = true;
    public static bool aguaSuficiente = true;

    public bool noiteFinalizada { get; private set; }

    public event Action<float> TimeChanged;
    public event Action<int> DayChanged;
    public event Action<DayState> DayStateChanged;

    private DayState currentDayState = DayState.Morning;
    private int ultimoMinuto = -1;

    public DayState CurrentDayState =>
        currentDayState;

    public override void _EnterTree()
    {
        if (Instance != null && Instance != this)
        {
            QueueFree();
            return;
        }

        Instance = this;
    }

    public override void _ExitTree()
    {
        if (Instance == this)
            Instance = null;
    }

    public override void _Ready()
    {
        currentDayState =
            GetDayState(minutoDoDia);

        ultimoMinuto =
            Mathf.FloorToInt(minutoDoDia);
    }

    public override void _Process(double delta)
    {
        WorldArea areaAtual =
            WorldManager.Instance?.AreaAtual;

        if (areaAtual == null)
            return;

        if (areaAtual.tempoParado)
            return;

        float duracaoDia =
            areaAtual.tempoAcelerado
                ? 120f
                : 1440f;

        float deltaF =
            (float)delta * escalaTempo;

        if (deltaF <= 0f)
            return;

        minutoDoDia +=
            deltaF * (1440f / duracaoDia);

        if (!noiteFinalizada &&
            minutoDoDia >= minutoFimNoite)
        {
            minutoDoDia = minutoFimNoite;
            noiteFinalizada = true;
            finalizarNoite();
            return;
        }

        if (minutoDoDia >= 1440f)
            minutoDoDia %= 1440f;

        int minutoAtual =
            Mathf.FloorToInt(minutoDoDia);

        if (minutoAtual != ultimoMinuto)
        {
            ultimoMinuto = minutoAtual;
            TimeChanged?.Invoke(
                minutoDoDia
            );
        }

        ChangeDayState();
    }

    private void ChangeDayState()
    {
        DayState novoEstado =
            GetDayState(minutoDoDia);

        if (novoEstado == currentDayState)
            return;

        currentDayState = novoEstado;

        DayStateChanged?.Invoke(
            currentDayState
        );
    }

    public DayState GetDayState(float minuto)
    {
        if (minuto >= 1200f)
            return DayState.Night;

        if (minuto >= 960f)
            return DayState.Evening;

        if (minuto >= 720f)
            return DayState.Afternoon;

        return DayState.Morning;
    }

    public void CongelarTempo()
    {
        escalaTempo = 0f;
        GameState.TempoCongelado = true;

        GameState.Instance.SetCameraInputEnabled(false);
        GameState.Instance.SetCameraMouseCaptured(false);
    }

    public void DescongelarTempo()
    {
        escalaTempo = 1f;
        GameState.TempoCongelado = false;

        GameState.Instance.SetCameraInputEnabled(true);
        GameState.Instance.SetCameraMouseCaptured(true);
    }

    public void finalizarNoite()
    {
        if (noiteFinalizada == false)
            noiteFinalizada = true;

        bool teveComida =
            NeedsState.Instance.varFome > 0f;

        bool teveAgua =
            NeedsState.Instance.varSede > 0f;

        comidaSuficiente = teveComida;
        aguaSuficiente = teveAgua;

        if (teveComida)
            diasSemComer = 0;
        else
            diasSemComer++;

        if (teveAgua)
            diasSemBeber = 0;
        else
            diasSemBeber++;

        bool temAbrigo =
            lugarParaDormir ||
            EffectManager.Instance.PermiteDormirSemLugar();

        if (temAbrigo)
            diasSemAbrigo = 0;
        else
            diasSemAbrigo++;

        EffectManager.Instance.FimDoDia();

        if (diasSemComer >= maximoDiasSemComer)
        {
            GameState.Instance.endGame(
                "semComida"
            );

            return;
        }

        if (diasSemBeber >= maximoDiasSemBeber)
        {
            GameState.Instance.endGame(
                "semAgua"
            );

            return;
        }

        if (diasSemAbrigo >= maximoDiasSemAbrigo)
        {
            GameState.Instance.endGame(
                "semLugarParaDormir"
            );

            return;
        }

        CongelarTempo();

        if (EffectManager.Instance.BloqueouNovasManutencoes)
            return;

        PackedScene cena =
            GD.Load<PackedScene>(
                "res://Assets/UI/escolhaEfeito.tscn"
            );

        escolhaEfeito tela =
            cena.Instantiate<escolhaEfeito>();

        GetTree().Root.AddChild(tela);

        tela.Abrir(
            EffectManager.Instance.GerarOpcoes()
        );
    }

    public void proximoDia()
    {
        diaAtual++;

        if (diaAtual > maximoDias)
            diaAtual = 1;

        minutoDoDia =
            minutoInicioDia;

        ultimoMinuto =
            Mathf.FloorToInt(
                minutoDoDia
            );

        noiteFinalizada = false;

        NeedsState.Instance.SetFome(0f);
        NeedsState.Instance.SetSede(0f);

        EffectManager.Instance.InicioDoDia();

        DayChanged?.Invoke(
            diaAtual
        );

        TimeChanged?.Invoke(
            minutoDoDia
        );

        ChangeDayState();

        DescongelarTempo();
    }

    public void alterarInicioDia(float novoInicio)
    {
        minutoInicioDia =
            Mathf.Clamp(
                novoInicio,
                0f,
                1440f
            );
    }

    public void alterarFimNoite(float novoFim)
    {
        minutoFimNoite =
            Mathf.Clamp(
                novoFim,
                0f,
                1440f
            );
    }

    public void AlterarMaximoDiasSemComer(int novoMaximo)
    {
        maximoDiasSemComer =
            Mathf.Max(
                0,
                novoMaximo
            );
    }

    public void AlterarMaximoDiasSemBeber(int novoMaximo)
    {
        maximoDiasSemBeber =
            Mathf.Max(
                0,
                novoMaximo
            );
    }

    public void AlterarMaximoDiasSemAbrigo(int novoMaximo)
    {
        maximoDiasSemAbrigo =
            Mathf.Max(
                0,
                novoMaximo
            );
    }
}

public enum DayState
{
    Morning,
    Afternoon,
    Evening,
    Night
}