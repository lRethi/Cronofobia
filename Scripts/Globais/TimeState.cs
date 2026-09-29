using Godot;
using System;

public partial class TimeState : Node
	{
	public static TimeState Instance { get; private set; }

	[Export]
	public float duracaoDiaSegundos = 120f;

	public float minutoDoDia { get; private set; } = 1080f;
	public float minutoInicioDia { get; private set; } = 480f;
	public float minutoFimNoite { get; private set; } = 1320f;

	public float horaDecimal => minutoDoDia / 60f;
	public float tempoNormalizado => minutoDoDia / 1440f;
	public int tempoHoras => (int)(minutoDoDia / 60f);
	public int tempoMinutos => (int)(minutoDoDia % 60f);
	public string horarioFormatado => $"{tempoHoras:D2}:{tempoMinutos:D2}";

	public float escalaTempo = 1f;

	public int diaAtual { get; private set; } = 1;
	public int maximoDias { get; private set; } = 7;

	public static bool lugarParaDormir = true;
	public static bool comidaSuficiente = true;
	public static bool aguaSuficiente = true;

	public bool noiteFinalizada { get; private set; } = false;

	public event Action<float> TimeChanged;
	public event Action<int> DayChanged;
	public event Action<DayState> DayStateChanged;

	private DayState currentDayState = DayState.Morning;
	private int ultimoMinuto = -1;

	public DayState CurrentDayState => currentDayState;

	public override void _EnterTree()
	{
		if (Instance != null && Instance != this)
		{
			QueueFree();
			return;
		}

		Instance = this;
	}

	public override void _Ready()
	{
		currentDayState = GetDayState(minutoDoDia);
		ultimoMinuto = Mathf.FloorToInt(minutoDoDia);
	}

	public override void _Process(double delta)
	{
		if (GetTree().CurrentScene?.SceneFilePath != "res://Assets/Scenes/ThePlayground.tscn")
        	return;

		float deltaF = (float)delta * escalaTempo;

		if (deltaF <= 0f)
			return;

		minutoDoDia += deltaF * (1440f / duracaoDiaSegundos);

		if (minutoDoDia >= 1440f)
			minutoDoDia %= 1440f;

		int minutoAtual = Mathf.FloorToInt(minutoDoDia);

		if (minutoAtual != ultimoMinuto)
		{
			ultimoMinuto = minutoAtual;
			TimeChanged?.Invoke(minutoDoDia);
		}

		if (!noiteFinalizada && minutoDoDia >= minutoFimNoite)
		{
			noiteFinalizada = true;
			finalizarNoite();
		}

		ChangeDayState();
	}

	private void ChangeDayState()
	{
		DayState novoEstado = GetDayState(minutoDoDia);

		if (novoEstado == currentDayState)
			return;

		currentDayState = novoEstado;
		DayStateChanged?.Invoke(currentDayState);
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
		EffectManager.Instance.FimDoDia();

		CongelarTempo();

		if (lugarParaDormir)
		{
			if (comidaSuficiente && aguaSuficiente)
			{
				PackedScene cena = GD.Load<PackedScene>("res://Assets/UI/escolhaEfeito.tscn");
				escolhaEfeito tela = cena.Instantiate<escolhaEfeito>();

				GetTree().Root.AddChild(tela);

				tela.Abrir(EffectManager.Instance.GerarOpcoes());
			}
			else
			{
				GameState.Instance.endGame("semComidaOuAgua");
			}
		}
		else
		{
			GameState.Instance.endGame("semLugarParaDormir");
		}
	}

	public void proximoDia()
	{
		diaAtual += 1;

		if (diaAtual > maximoDias)
			diaAtual = 1;

		minutoDoDia = minutoInicioDia;
		ultimoMinuto = Mathf.FloorToInt(minutoDoDia);

		EffectManager.Instance.InicioDoDia();

		noiteFinalizada = false;

		NeedsState.Instance.SetFome(0f);
		NeedsState.Instance.SetSede(0f);

		DayChanged?.Invoke(diaAtual);
		TimeChanged?.Invoke(minutoDoDia);

		ChangeDayState();
		DescongelarTempo();
	}

	public void alterarInicioDia(float novoInicio)
	{
		minutoInicioDia = Mathf.Clamp(novoInicio, 0f, 1440f);
	}

	public void alterarFimNoite(float novoFim)
	{
		minutoFimNoite = Mathf.Clamp(novoFim, 0f, 1440f);
	}

	}

	public enum DayState
	{
		Morning,
		Afternoon,
		Evening,
		Night
	}
