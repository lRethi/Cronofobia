using Godot;

public partial class OPENTHEGATES : StaticBody3D
{
    [Export] public WorldArea targetArea;
    [Export] public CharacterBody3D personagem;
    [Export] public bool timeLimit = false;
    [Export] public float openingTime = 0f;
    [Export] public float closingTime = 0f;
    [Export] public bool isInterior = false;
    [Export] public string cantInteractFlag = "";
    private bool podeInteragir = true;
    private bool playerPerto = false;

    public override void _Ready()
    {
        if (personagem != null)
            return;

        Node area = GetParent();

        while (area != null)
        {
            CharacterBody3D encontrado =
                area.FindChild("charGeraldoSalvador", true, false)
                as CharacterBody3D;

            if (encontrado != null)
            {
                personagem = encontrado;
                break;
            }

            area = area.GetParent();
        }
    }

    public override void _Process(double delta)
    {
        if (personagem == null || TimeState.Instance == null)
            return;

        if (GameState.Instance.GetFlag(cantInteractFlag))
        {
            if (isInterior) TrocarArea();
            else return;
        } 

        float minutoAtual = TimeState.Instance.minutoDoDia;

        if (!Input.IsActionPressed("interact"))
            podeInteragir = true;

        if (timeLimit && minutoAtual > closingTime)
        {
            if (isInterior && podeInteragir)
            {
                podeInteragir = false;
                TrocarArea();
            }

            return;
        }

        if (timeLimit && minutoAtual < openingTime)
            return;

        float distancia =
            (personagem.GlobalPosition - GlobalPosition).Length();

        playerPerto = distancia <= 1.25f;

        if (playerPerto &&
            Input.IsActionJustPressed("interact") &&
            podeInteragir)
        {
            podeInteragir = false;
            TrocarArea();
        }
    }

    public void TrocarArea()
    {
        if (targetArea == null || WorldManager.Instance == null)
            return;

        CallDeferred(nameof(ExecutarTrocaArea));
    }

    private void ExecutarTrocaArea()
    {
        if (targetArea == null || WorldManager.Instance == null)
            return;

        WorldManager.Instance.AtivarArea(targetArea);
    }
}