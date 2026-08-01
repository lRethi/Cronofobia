using Godot;

public partial class TimeState : Node
{
	public static TimeState Instance { get; private set; }

	[Export] public float duracaoDiaSegundos = 120f;
	public float minutoDoDia {get; set;} = 1300f; // 0 -> 1440

	public float minutoInicioDia {get; private set;} = 480f; // 08:00

	public float minutoFimNoite {get; private set;} = 1320f; // 22:00	
	public float horaDecimal => minutoDoDia / 60f; // 0 -> 24

	public float tempoNormalizado => minutoDoDia / 1440f; // 0 -> 1

	public int tempoHoras => (int)(minutoDoDia / 60);
	public int tempoMinutos => (int)(minutoDoDia % 60);

	public string horarioFormatado => $"{tempoHoras:D2}:{tempoMinutos:D2}";
	public float escalaTempo = 1f;

	public float diaAtual {private set; get;} = 1f;
	public float maximoDias {private set; get;} = 7f;

	public static bool lugarParaDormir = true;
    public static bool comidaSuficiente = true;
    public static bool aguaSuficiente = true;
	public bool noiteFinalizada {get; private set;} = false;

	public override void _EnterTree()
		{
			if (Instance != null && Instance != this) {
			QueueFree();
			return;
		}
		Instance = this;
	}

	public override void _Process(double delta)
	{
		float deltaF = (float)delta * escalaTempo;

		minutoDoDia += (deltaF * escalaTempo) * (1440f / duracaoDiaSegundos);

		if (!noiteFinalizada && minutoDoDia >= minutoFimNoite)
		{
			noiteFinalizada = true;
			finalizarNoite();
		}
	}

	public void CongelarTempo()
	{
		escalaTempo = 0f;
		GameState.TempoCongelado = true;
	}

	public void DescongelarTempo()
	{
		escalaTempo = 1f;
		GameState.TempoCongelado = false;
	}
	public void finalizarNoite()
	{
		EffectManager.Instance.FimDoDia();
		CongelarTempo();
		if(lugarParaDormir)
		{
			if(comidaSuficiente && aguaSuficiente)
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
		diaAtual += 1f;
		if (diaAtual > maximoDias)
			diaAtual = 1f;
		minutoDoDia = minutoInicioDia;
		EffectManager.Instance.InicioDoDia();
		noiteFinalizada = false;
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