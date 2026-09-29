using Godot;

public partial class LoadingScreen : Control
{
    public async void IniciarFinalizacao()
    {

        await ToSignal(
            RenderingServer.Singleton,
            RenderingServer.SignalName.FramePostDraw
        );

        QueueFree();
    }
}