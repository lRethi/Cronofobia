using Godot;

public partial class rotCameraPerson : Sprite3D
{
    ShaderMaterial mat;
    Camera3D camera;

    [Export] public bool rotateToPerson = true;

    [Export] CharacterBody3D personagem;

    public override void _Ready()
    {
        personagem = GetNode<CharacterBody3D>("../../../%charGeraldoSalvador");

        camera = GetViewport().GetCamera3D();

        if (MaterialOverlay != null)
        {
            MaterialOverlay = MaterialOverlay.Duplicate() as Material;
            mat = MaterialOverlay as ShaderMaterial;
        }
    }

    public override void _Process(double delta)
    {
        if (personagem == null || camera == null || mat == null)
            return;

        Vector3 direcaoCamera =
            camera.GlobalPosition - GlobalPosition;

        OlharParaAlvo(direcaoCamera);

        Vector3 distanciaPersonNPC =
            personagem.GlobalPosition - GlobalPosition;

        bool podeInteragir =
            distanciaPersonNPC.Length() <= 1.25f;

        bool mostrarOutline =
            podeInteragir &&
            !GameState.Instance.DialogoAberto;

        mat.SetShaderParameter(
            "enable_outline",
            mostrarOutline
        );
    }

    public void OlharParaAlvo(Vector3 alvoDir)
    {
        if(!rotateToPerson) return;
        
        alvoDir.Y = 0;
        alvoDir = -alvoDir;

        if (GetParent() is Node3D parentNode)
        {
            parentNode.LookAt(
                parentNode.GlobalPosition + alvoDir,
                Vector3.Up
            );
        }
    }
}