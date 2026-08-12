using Godot;

[GlobalClass]
public partial class DirectionalSpriteData : Resource
{
    [Export]
    public Texture2D Texture { get; set; }

    [Export]
    public Texture2D NormalMap { get; set; }

    [Export]
    public Texture2D Emission { get; set; }
}