using Godot;

public partial class TimeState : Node
{
	public static TimeState Instance { get; private set; }

	[Export] public float duracaoDiaSegundos = 120f;

	public float minutoDoDia {get; set;} = 480f; // 0 -> 1440
	public float horaDecimal => minutoDoDia / 60f; // 0 -> 24

	public float tempoNormalizado => minutoDoDia / 1440f; // 0 -> 1

	public int tempoHoras => (int)(minutoDoDia / 60);
	public int tempoMinutos => (int)(minutoDoDia % 60);

	public string horarioFormatado => $"{tempoHoras:D2}:{tempoMinutos:D2}";
	public float escalaTempo = 1f;

	public override void _EnterTree()
	{
		Instance = this;
	}

	public override void _Process(double delta)
	{
		float deltaF = (float)delta * escalaTempo;

		minutoDoDia += (deltaF * escalaTempo) * (1440f / duracaoDiaSegundos);

		if (minutoDoDia >= 1440f)
			minutoDoDia -= 1440f;
	}

	public void CongelarTempo()
	{
		escalaTempo = 0f;
	}

	public void DescongelarTempo()
	{
		escalaTempo = 1f;
	}
}