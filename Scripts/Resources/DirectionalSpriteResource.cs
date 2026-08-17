using Godot;

[GlobalClass]
public partial class DirectionalSpriteResource : Resource
{
    [Export]
    public DirectionalSpriteData Back { get; set; }

    [Export]
    public DirectionalSpriteData Right { get; set; }

    [Export]
    public DirectionalSpriteData Front { get; set; }

    [Export]
    public DirectionalSpriteData Left { get; set; }

    [Export]
    public bool CastShadow { get; set; } = true;

    [Export]
    public bool GlowEnabled { get; set; } = false;

    [Export]
    public Color GlowBaseColor { get; set; } = Color.FromHtml("#bdba97");

    [Export]
    public float GlowTolerance { get; set; } = 0.15f;

    [Export]
    public float GlowStrength { get; set; } = 4f;
}