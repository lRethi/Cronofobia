using Godot;

public partial class teleportPlayer : Area3D
{
    [Export] public Node3D referenciaTeleport;

    [Export] public bool mapSwitch = false;

    [Export] public WorldArea targetArea;

    public override void _Ready()
    {
        BodyEntered += AoEntrar;
    }

    private void AoEntrar(Node3D body)
    {
        if (body is movimentoPerson personagem)
        {
                personagem.GlobalPosition = referenciaTeleport.GlobalPosition;
                personagem.Velocity = Vector3.Zero;
            if (mapSwitch) TrocarArea();
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