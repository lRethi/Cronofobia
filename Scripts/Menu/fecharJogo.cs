using Godot;

public partial class fecharJogo : Button
{
    public override void _Ready()
    {
        Pressed += Fechar;
    }

    private void Fechar()
    {
        GetTree().Quit();
    }
}