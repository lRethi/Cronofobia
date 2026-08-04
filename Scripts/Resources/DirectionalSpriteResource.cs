using Godot;
using System;

[GlobalClass]
public partial class DirectionalSpriteResource : Resource
{
    [Export] public Texture2D Back;
    [Export] public Texture2D Right;
    [Export] public Texture2D Front;
    [Export] public Texture2D Left;
}
