using Godot;
using System;
using System.Collections.Generic;
using Godot.Collections;
using System.Linq;

public partial class EffectManager : Node
{
	public static EffectManager Instance { get; private set; }
	private List<EffectBase> efeitosDisponiveis = new();
	private List<EffectBase> efeitosAtivos = new();

    public override void _EnterTree()
		{
			if (Instance != null && Instance != this) {
			QueueFree();
			return;
		}
        Instance = this;
    }
	public override void _Ready()
	{
		efeitosDisponiveis.Add(new MaisFomeMenosSede());
		efeitosDisponiveis.Add(new MaisSedeMenosFome());
		efeitosDisponiveis.Add(new DinheiroEmComida());
	}

	public void AdicionarEfeito(EffectBase efeito)
	{
		efeitosAtivos.Add(efeito);
		efeito.aoEscolher();
	}
	public List<EffectBase> GerarOpcoes()
	{
		List<EffectBase> copiaEfeitos = new List<EffectBase>(efeitosDisponiveis);
		copiaEfeitos.Shuffle();
		return copiaEfeitos.Take(3).ToList();
	}
	public void InicioDoDia()
	{
		foreach (var efeito in efeitosAtivos)
		{
			efeito.InicioDoDia();
		}
	}
	public void FimDoDia()
	{
		foreach (var efeito in efeitosAtivos)
		{
			efeito.FimDoDia();
		}
	}
}
	public static class ListExtensions
	{
		public static void Shuffle<T>(this IList<T> list)
		{
			Random random = Random.Shared;

			for (int i = list.Count - 1; i > 0; i--)
			{
				int j = random.Next(i + 1);

				(list[i], list[j]) = (list[j], list[i]);
			}
		}
	}