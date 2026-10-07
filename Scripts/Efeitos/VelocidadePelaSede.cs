using Godot;

public partial class VelocidadePelaSede : EffectBase
{
    public override string Nome => "Velocidade pela Sede";

    public override string Desc =>
        "Quanto mais sede você tiver, mais rápido anda.";

    public override string SpritePath =>
        "res://Assets/Sprites/Placeholder/the_placeholder.png";

    public override float BonusVelocidade()
    {
        int sede = Mathf.Clamp(
            Mathf.RoundToInt(
                NeedsState.Instance.varSede
            ),
            0,
            3
        );

        return sede switch
        {
            0 => 1f,
            1 => 0f,
            2 => -1f,
            _ => -2f
        };
    }
}