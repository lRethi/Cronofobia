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
        if (GameState.Instance != null)
        {
            GameState.Instance.FlagChanged += OnFlagChanged;

            if (GameState.Instance.GetFlag(flagRoubo))
            {
                Alertar();
            }
        }
    }

    public override void _ExitTree()
    {
        if (GameState.Instance != null)
        {
            GameState.Instance.FlagChanged -= OnFlagChanged;
        }
    }

    private void OnFlagChanged(string flag, bool valor)
    {
        if (flag != flagRoubo)
        {
            return;
        }

        if (!valor)
        {
            return;
        }

        Alertar();
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

        alertaAtivada = true;

        Node3D playerAtual = ObterPlayerAtual();

        if (playerAtual == null)
        {
            return;
        }

        SpawnInimigo(pontoSpawn1, playerAtual);
        SpawnInimigo(pontoSpawn2, playerAtual);
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

        return WorldManager.Instance.AreaAtual.personagem;
    }

    private void SpawnInimigo(Node3D pontoSpawn, Node3D playerAtual)
    {
        if (pontoSpawn == null)
        {
            return;
        }

        var inimigo = cenaInimigo.Instantiate<inimPerseguirProta>();

        AddChild(inimigo);

        inimigo.GlobalTransform = pontoSpawn.GlobalTransform;
        inimigo.player = playerAtual;

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

        int capturasRestantes = TimeState.Instance.RegistrarCaptura();

        DestruirInimigos();

        if (capturasRestantes > 0)
        {
            TimeState.Instance.finalizarNoite(true);
            return;
        }

        GameState.Instance.endGame("endingConfinamento");
    }

    private void DestruirInimigos()
    {
        foreach (var inimigo in inimigos)
        {
            if (IsInstanceValid(inimigo))
            {
                inimigo.QueueFree();
            }
        }

        inimigos.Clear();
    }
}