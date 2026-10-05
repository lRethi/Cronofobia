public partial class TenisDinamico : EffectBase
{
    public override string Nome => "Tênis Dinâmico";

    public override string Desc =>
        "Ande 25% mais rápido, mas tenha 1 a mais de fome máxima e 1 a mais de sede máxima.";

    public override string SpritePath =>
        "res://Assets/Sprites/Placeholder/the_placeholder.png";

    public override float MultiplicadorVelocidade()
    {
        return 1.25f;
    }

    public override void aoEscolher()
    {
        NeedsState.Instance.AlterarMaximoFome(
            NeedsState.Instance.maximoFome + 1f
        );

        NeedsState.Instance.AlterarMaximoSede(
            NeedsState.Instance.maximoSede + 1f
        );
    }
}