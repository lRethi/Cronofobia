using Godot;

public partial class teleportPlayer : Area3D
{
    [Export] public Node3D referenciaTeleport;

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
        }
    }
}