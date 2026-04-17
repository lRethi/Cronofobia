using Godot;

public partial class TimeState : Node
{
	public static TimeState Instance { get; private set; }

	[Export] public float duracaoDiaSegundos = 60f;

	public float tempoNormalizado { get; private set; } = 0f; // 0 → 1
	public float tempoHoras => tempoNormalizado * 24f;
	public float escalaTempo = 1f;

	public override void _EnterTree()
	{
		Instance = this;
	}

	public override void _Process(double delta)
	{
		float deltaF = (float)delta * escalaTempo;

		tempoNormalizado += deltaF / duracaoDiaSegundos;

		if (tempoNormalizado >= 1f)
			tempoNormalizado -= 1f;
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