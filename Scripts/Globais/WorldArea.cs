using Godot;

public partial class WorldArea : Node3D
{
    [Export] public bool tempoAcelerado = false;
    [Export] public bool tempoParado = false;
    [Export] public CharacterBody3D personagem;
    [Export] public cameraBonitaDoFred camera;

    public void Ativar()
{
    Visible = true;
    ProcessMode = Node.ProcessModeEnum.Pausable;

    GD.Print("BANCO ROOT: ", GetPath(), " | Visible: ", Visible);

    foreach (Node filho in GetChildren())
    {
        if (filho is Node3D node3D)
        {
            GD.Print(
                "BANCO FILHO: ",
                node3D.GetPath(),
                " | Visible: ",
                node3D.Visible
            );
        }
    }

    if (personagem != null)
        personagem.ProcessMode = Node.ProcessModeEnum.Pausable;

    if (camera != null)
    {
        camera.ProcessMode = Node.ProcessModeEnum.Pausable;
        camera.AtivarCamera();
    }
}

    public void Desativar()
    {
        GD.Print(
            "AREA DESATIVAR | ",
            GetPath(),
            " | ANTES Visible: ",
            Visible,
            " | ProcessMode: ",
            ProcessMode
        );

        if (camera != null)
            camera.DesativarCamera();

        if (personagem != null)
            personagem.ProcessMode = Node.ProcessModeEnum.Disabled;

        ProcessMode = Node.ProcessModeEnum.Disabled;
        Visible = false;

        GD.Print(
            "AREA DESATIVAR | ",
            GetPath(),
            " | DEPOIS Visible: ",
            Visible,
            " | ProcessMode: ",
            ProcessMode
        );
    }
}