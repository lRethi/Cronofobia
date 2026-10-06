using Godot;

public partial class rotCameraPerson : Sprite3D
{
    ShaderMaterial mat;
    Camera3D camera;

    [Export] public bool rotateToPerson = true;
    [Export] CharacterBody3D personagem;

    [Export]
    public float alcanceInteracao = 1.25f;

    public override void _Ready()
    {
        if (personagem == null) personagem = GetTree().CurrentScene.FindChild("charGeraldoSalvador", true, false) as CharacterBody3D;
        camera = GetViewport().GetCamera3D();

        if (MaterialOverlay != null)
        {
            MaterialOverlay = MaterialOverlay.Duplicate() as Material;
            mat = MaterialOverlay as ShaderMaterial;
        }
    }

    public override void _Process(double delta)
    {
        if (personagem == null || camera == null || mat == null || !GetParentNode3D().Visible)
            return;

        Node atual = GetParent();

        while (atual != null && atual is not OPENTHEGATES)
            atual = atual.GetParent();

        if (atual is OPENTHEGATES openGate && openGate.timeLimit)
        {
            if (TimeState.Instance.minutoDoDia < openGate.openingTime || TimeState.Instance.minutoDoDia > openGate.closingTime)
                return;
        }

        Vector3 direcaoCamera =
            camera.GlobalPosition - GlobalPosition;

        OlharParaAlvo(direcaoCamera);

        Vector3 distanciaPersonNPC =
            personagem.GlobalPosition - GlobalPosition;

        float alcance =
            EffectManager.Instance != null
                ? EffectManager.Instance.GetInteractionRange(
                    alcanceInteracao
                )
                : alcanceInteracao;

        bool podeInteragir =
            distanciaPersonNPC.Length() <= alcance;

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
        if (!rotateToPerson)
            return;

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