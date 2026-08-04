using Godot;

public partial class lampPostController : Node
{
    [Export]
    public DirectionalSprite directionalSprite;

    [Export]
    public DirectionalSpriteResource offSprites;

    [Export]
    public DirectionalSpriteResource onSprites;

    [Export]
    public OmniLight3D light;

    public override void _Ready()
    {
        TimeState.Instance.DayStateChanged += OnDayStateChanged;

        OnDayStateChanged(TimeState.Instance.CurrentDayState);
    }

    public override void _ExitTree()
    {
        if (TimeState.Instance != null)
            TimeState.Instance.DayStateChanged -= OnDayStateChanged;
    }

    private void OnDayStateChanged(DayState state)
    {
        bool aceso =
            state == DayState.Evening ||
            state == DayState.Night;

        light.Visible = aceso;

        directionalSprite.SetSpriteSet(
            aceso ? onSprites : offSprites
        );
    }
}