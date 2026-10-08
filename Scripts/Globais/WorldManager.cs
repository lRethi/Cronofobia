using Godot;
using System;

public partial class WorldManager : Node
{
    public static WorldManager Instance { get; private set; }

    [Export] public WorldArea areaInicial;
    [Export] public WorldArea[] areas = Array.Empty<WorldArea>();

    public WorldArea AreaAtual { get; private set; }

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
        foreach (WorldArea area in areas)
        {
            if (area == null || area == areaInicial)
                continue;

            area.Desativar();
        }

        if (areaInicial != null)
            AtivarArea(areaInicial);
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        if (@event.IsActionPressed("interrupt"))
            GameState.Instance?.voltarParaOMenu();
    }

    public void AtivarArea(WorldArea novaArea)
    {
        if (novaArea == null)
            return;

        if (AreaAtual == novaArea)
            return;

        WorldArea areaAnterior = AreaAtual;

        AreaAtual = novaArea;
        AreaAtual.Ativar();

        if (AreaAtual.camera != null)
            AreaAtual.camera.AtivarCamera();

        if (areaAnterior != null)
            areaAnterior.Desativar();
    }

    public void Resetar()
    {
        AreaAtual = null;
    }
}