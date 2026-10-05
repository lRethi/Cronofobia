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
            if (area == null)
                continue;

            if (area == areaInicial)
                continue;

            area.Desativar();
        }

        if (areaInicial != null)
            AtivarArea(areaInicial);
    }

    public override void _Process(double delta)
    {
        if (AreaAtual == null)
            return;

        if (!AreaAtual.Visible)
        {
            GD.Print(
                "AREA SUMIU | ",
                AreaAtual.GetPath(),
                " | ProcessMode: ",
                AreaAtual.ProcessMode
            );
        }
    }

    public void AtivarArea(WorldArea novaArea)
    {
        if (novaArea == null)
            return;

        if (AreaAtual == novaArea)
            return;

        if (AreaAtual != null)
            AreaAtual.Desativar();

        AreaAtual = novaArea;
        AreaAtual.Ativar();

        GD.Print(
            "AREA APOS ATIVAR | ",
            AreaAtual.GetPath(),
            " | Visible: ",
            AreaAtual.Visible
        );

        CallDeferred(nameof(VerificarAreaAtiva));
    }

    private void VerificarAreaAtiva()
    {
        if (AreaAtual == null)
            return;

        GD.Print(
            "AREA NO FRAME SEGUINTE | ",
            AreaAtual.GetPath(),
            " | Visible: ",
            AreaAtual.Visible,
            " | ProcessMode: ",
            AreaAtual.ProcessMode
        );
    }
}