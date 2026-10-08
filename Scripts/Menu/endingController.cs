using Godot;

public partial class endingController : Node2D
{
    [Export] public AnimationPlayer animationPlayer;

    private bool endingFinalizado;

    public override void _Ready()
    {
        animationPlayer.AnimationFinished += OnAnimationFinished;
        animationPlayer.Play("endingSequence");
    }

    private void OnAnimationFinished(StringName animationName)
    {
        if (animationName == "endingSequence")
            endingFinalizado = true;
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        if (!endingFinalizado)
            return;

        if (@event.IsActionPressed("interrupt") || @event.IsActionPressed("interact"))
            VoltarAoMenu();
    }

    private void VoltarAoMenu()
    {
        GameState.Instance?.voltarParaOMenu();
    }
}