using Godot;

[GlobalClass]
public partial class ItemDefinition : Resource
{
    [Export] public string Id { get; set; } = "";
    [Export] public string Nome { get; set; } = "";
    [Export(PropertyHint.MultilineText)] public string Descricao { get; set; } = "";
    [Export] public Texture2D Textura { get; set; }
    [Export] public bool PodeSerUsado { get; set; } = true;
    [Export] public bool PodeSerDescartado { get; set; } = true;
    [Export] public float FomeAoUsar { get; set; } = 0f;
    [Export] public float SedeAoUsar { get; set; } = 0f;
}