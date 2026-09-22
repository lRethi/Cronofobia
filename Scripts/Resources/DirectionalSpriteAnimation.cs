using Godot;

[GlobalClass]
public partial class DirectionalSpriteAnimation : Resource
{
    [Export]
    public DirectionalSpriteResource[] Frames { get; set; } = [];
}