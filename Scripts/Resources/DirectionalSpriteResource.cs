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
    public bool Specular { get; set; } = false;

    [Export]
    public float Metallic { get; set; } = 0f;

    [Export]
    public float Roughness { get; set; } = 1f;

    [Export]
    public float EmissionEnergy { get; set; } = 1f;

    [Export]
    public Color EmissionColor { get; set; } = Colors.White;

    [Export]
    public bool CastShadow { get; set; } = true;
}