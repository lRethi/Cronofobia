using Godot;

public partial class safeZoneScript : Area3D
{
    [Export] public CharacterBody3D personagem;

    public override void _Ready()
    {
        BodyEntered += OnBodyEntered;
        BodyExited += OnBodyExited;
    }

    private void OnBodyEntered(Node3D body)
    {
        if (body == personagem)
        {
            TimeState.Instance.LugarSeguroParaDormir(true);
            GD.Print("Entrou em uma área segura.");
        }
    }

    private void OnBodyExited(Node3D body)
    {
        if (body == personagem)
        {
            TimeState.Instance.LugarSeguroParaDormir(false);
            GD.Print("Saiu de uma área segura.");
        }
    }
}