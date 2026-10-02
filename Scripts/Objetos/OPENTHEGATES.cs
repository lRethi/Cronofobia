using Godot;

using System;

public partial class OPENTHEGATES : StaticBody3D
{
    [Export] public PackedScene targetScene;
    [Export] public CharacterBody3D personagem;
    [Export] public bool timeLimit = false;
    [Export] public float openingTime = 0f;
    [Export] public float closingTime = 0f;

    private bool playerPerto = false;

    public override void _Process(double delta)
    {
        if (personagem == null || (timeLimit && (TimeState.Instance.minutoDoDia < openingTime || TimeState.Instance.minutoDoDia > closingTime)))
            return;

        float distancia = (personagem.GlobalPosition - GlobalPosition).Length();
        playerPerto = distancia <= 1.25f;

        if (playerPerto && Input.IsActionJustPressed("interact"))
        {
            TrocarCena();
        }
    }

    public void TrocarCena()
    {
        if (targetScene == null)
            return;

        GetTree().ChangeSceneToPacked(targetScene);
    }
}