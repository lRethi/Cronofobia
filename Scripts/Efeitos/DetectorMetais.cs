public partial class DetectorMetais : EffectBase
{
    public override string Nome => "Detector de Metais";

    public override string Desc =>
        "Água não aparece mais no chão, mas moedas aparecem duas vezes mais.";

    public override string SpritePath =>
        "res://Assets/Sprites/Placeholder/the_placeholder.png";

    public override void aoEscolher()
    {
        GameState.Instance.SetFlag(
            "detectorMetais",
            true
        );
    }
}