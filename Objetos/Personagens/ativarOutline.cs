using Godot;
using System;

public partial class ativarOutline : Sprite3D
{
    [Export] CharacterBody3D personagem;

    ShaderMaterial mat;

    public override void _Ready()
    {
        if (MaterialOverride != null)
        {
            MaterialOverride = MaterialOverride.Duplicate() as Material;
        }

        mat = MaterialOverride as ShaderMaterial;
    }

    public override void _Process(double delta)
    {
        if (personagem == null || mat == null)
            return;

        float distancia = (personagem.GlobalPosition - GlobalPosition).Length();

        bool perto = distancia <= 2f;

        mat.SetShaderParameter("enable_outline", perto);
    }
}