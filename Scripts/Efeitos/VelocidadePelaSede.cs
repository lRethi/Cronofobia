using Godot;

public partial class VelocidadePelaSede : EffectBase
{
    public override string Nome => "Velocidade pela Sede";

    public override string Desc =>
        "Quanto mais sede você tiver, mais devagar anda. Sede 0 recebe +1 de velocidade, sede 1 não recebe modificador, sede 2 recebe -1 e sede 3 recebe -2.";

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