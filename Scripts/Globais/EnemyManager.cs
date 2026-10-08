using Godot;
using System.Collections.Generic;

public partial class EnemyManager : Node3D
{
	[Export] public string flagRoubo = "roubouLoja1";
	[Export] public PackedScene cenaInimigo;
	[Export] public Node3D pontoSpawn1;
	[Export] public Node3D pontoSpawn2;
	[Export] public Node3D pontoCaptura;
	[Export] public Node3D player;

	private bool alertaAtivada = false;
	private bool capturaEmAndamento = false;
	private readonly List<inimPerseguirProta> inimigos = new();

	public override void _Ready()
	{
		if (GameState.Instance == null)
		{
			return;
		}

		GameState.Instance.FlagChanged += OnFlagChanged;
		CallDeferred(nameof(VerificarRouboInicial));
	}

	public override void _ExitTree()
	{
		if (GameState.Instance != null)
		{
			GameState.Instance.FlagChanged -= OnFlagChanged;
		}
	}

	private void VerificarRouboInicial()
	{
		if (GameState.Instance == null)
		{
			return;
		}

		if (!GameState.Instance.GetFlag(flagRoubo))
		{
			return;
		}

		Alertar();
	}

	private void OnFlagChanged(string flag, bool valor)
    {
        if (flag != flagRoubo)
        {
            return;
        }

        if (!valor)
        {
            alertaAtivada = false;
            capturaEmAndamento = false;
            DestruirInimigos();
            return;
        }

        CallDeferred(nameof(Alertar));
    }

	private void Alertar()
	{
		if (alertaAtivada)
		{
			return;
		}

		if (cenaInimigo == null)
		{
			return;
		}

		Node3D playerAtual = ObterPlayerAtual();

		if (playerAtual == null)
		{
			return;
		}

		bool algumInimigoSpawnado = false;

		if (pontoSpawn1 != null)
		{
			SpawnInimigo(pontoSpawn1, playerAtual);
			algumInimigoSpawnado = true;
		}

		if (pontoSpawn2 != null)
		{
			SpawnInimigo(pontoSpawn2, playerAtual);
			algumInimigoSpawnado = true;
		}

		if (!algumInimigoSpawnado)
		{
			return;
		}

		alertaAtivada = true;
	}

	private Node3D ObterPlayerAtual()
	{
		if (IsInstanceValid(player))
		{
			return player;
		}

		if (WorldManager.Instance == null)
		{
			return null;
		}

		if (WorldManager.Instance.AreaAtual == null)
		{
			return null;
		}

		WorldArea areaAtual = WorldManager.Instance.AreaAtual;

		if (!IsInstanceValid(areaAtual.personagem))
		{
			return null;
		}

		return areaAtual.personagem;
	}

	private void SpawnInimigo(Node3D pontoSpawn, Node3D playerAtual)
	{
		if (pontoSpawn == null || cenaInimigo == null || playerAtual == null)
		{
			return;
		}

		inimPerseguirProta inimigo = cenaInimigo.Instantiate<inimPerseguirProta>();

		inimigo.player = playerAtual;

		AddChild(inimigo);

		inimigo.GlobalTransform = pontoSpawn.GlobalTransform;
		inimigo.PlayerColidiu += OnPlayerColidiu;

		inimigos.Add(inimigo);
	}

	private void OnPlayerColidiu()
	{
		if (capturaEmAndamento)
		{
			return;
		}

		capturaEmAndamento = true;

		Node3D playerAtual = ObterPlayerAtual();

		if (playerAtual != null && pontoCaptura != null)
		{
			playerAtual.GlobalPosition = pontoCaptura.GlobalPosition;
		}

		if (TimeState.Instance == null)
		{
			DestruirInimigos();
			return;
		}

		int capturasRestantes = TimeState.Instance.RegistrarCaptura();

		DestruirInimigos();

		if (capturasRestantes > 0)
		{
			TimeState.Instance.finalizarNoite(true);
			return;
		}

		if (GameState.Instance != null)
		{
			GameState.Instance.endGame("confinamento");
		}
	}

	private void DestruirInimigos()
	{
		foreach (inimPerseguirProta inimigo in inimigos)
		{
			if (IsInstanceValid(inimigo))
			{
				inimigo.QueueFree();
			}
		}

		inimigos.Clear();
	}
}