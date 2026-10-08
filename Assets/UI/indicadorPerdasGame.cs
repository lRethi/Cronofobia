using Godot;
using System;

public partial class indicadorPerdasGame : TextureRect
{
    [Export] public bool isFome;
    [Export] public bool isSede;
    [Export] public bool isSleep;
    [Export] public bool isPrison;

	[Export]
	public int maximoDiasSemComer { get; private set; } = 2;

	[Export]
	public int maximoDiasSemBeber { get; private set; } = 2;

    public override void _Ready()
    {
        Modulate = new Color(1, 1, 1, 0);
    }

    public override void _Process(double delta)
    {
        bool perigo = false;

        if (isFome)
        {
            perigo = TimeState.Instance.diasSemComer >= maximoDiasSemComer - 1;
        }

        if (isSede)
        {
            perigo = TimeState.Instance.diasSemBeber >= maximoDiasSemBeber - 1;
        }

        if (isSleep)
        {
            perigo = TimeState.Instance.diasSemAbrigo >= TimeState.Instance.maximoDiasSemAbrigo - 1;
        }

        if (isPrison)
        {
            perigo = TimeState.Instance.CapturasRestantes <= 1;
        }

        Modulate = perigo ? new Color(1, 1, 1, 1) : new Color(1, 1, 1, 0);
    }
}