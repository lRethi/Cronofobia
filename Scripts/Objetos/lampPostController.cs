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

    private Tween flickerTween;

    private readonly RandomNumberGenerator rng = new();

    private bool isOn;

    public override void _Ready()
    {
        rng.Randomize();

        TimeState.Instance.DayStateChanged += OnDayStateChanged;

        OnDayStateChanged(TimeState.Instance.CurrentDayState);
    }

    public override void _ExitTree()
    {
        flickerTween?.Kill();

        if (TimeState.Instance != null)
            TimeState.Instance.DayStateChanged -= OnDayStateChanged;
    }

    private void OnDayStateChanged(DayState state)
    {
        bool aceso =
            state == DayState.Evening ||
            state == DayState.Night;

        flickerTween?.Kill();

        if (!aceso)
        {
            isOn = false;

            light.Visible = false;

            directionalSprite.SetSpriteSet(offSprites);
            directionalSprite.SetGlowEnabled(false);

            return;
        }

        isOn = true;

        directionalSprite.SetSpriteSet(onSprites);

        StartFlicker();
    }

    private async void StartFlicker()
    {
        light.Visible = false;
        directionalSprite.SetGlowEnabled(false);

        int flickers = rng.RandiRange(2, 4);

        for (int i = 0; i < flickers; i++)
        {
            if (!isOn)
                return;

            light.Visible = true;
            directionalSprite.SetGlowEnabled(true);

            await ToSignal(
                GetTree().CreateTimer(
                    rng.RandfRange(0.05f, 0.15f)
                ),
                SceneTreeTimer.SignalName.Timeout
            );

            if (!isOn)
                return;

            light.Visible = false;
            directionalSprite.SetGlowEnabled(false);

            await ToSignal(
                GetTree().CreateTimer(
                    rng.RandfRange(0.05f, 0.2f)
                ),
                SceneTreeTimer.SignalName.Timeout
            );
        }

        if (!isOn)
            return;

        light.Visible = true;
        directionalSprite.SetGlowEnabled(true);
    }
}