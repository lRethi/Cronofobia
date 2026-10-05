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

        if (personagem != null)
            personagem.ProcessMode = Node.ProcessModeEnum.Pausable;

        if (camera != null)
            camera.ProcessMode = Node.ProcessModeEnum.Pausable;
    }

    public void Desativar()
    {
        if (camera != null)
            camera.DesativarCamera();

        if (personagem != null)
            personagem.ProcessMode = Node.ProcessModeEnum.Disabled;

        ProcessMode = Node.ProcessModeEnum.Disabled;
        Visible = false;
    }
}