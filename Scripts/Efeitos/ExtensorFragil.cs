public partial class ExtensorFragil : EffectBase
{
    public override string Nome => "Extensor Frágil";

    public override string Desc =>
        "Aumenta em 2 vezes o alcance de interação com NPCs e itens, mas reduz em 1 o limite máximo de dias sem se alimentar e sem beber.";

    public override string SpritePath =>
        "res://Assets/Sprites/Placeholder/the_placeholder.png";

    public override float MultiplicadorAlcanceInteracao()
    {
        return 2f;
    }

    public override void aoEscolher()
    {
        TimeState.Instance.AlterarMaximoDiasSemComer(
            TimeState.Instance.maximoDiasSemComer - 1
        );

        TimeState.Instance.AlterarMaximoDiasSemBeber(
            TimeState.Instance.maximoDiasSemBeber - 1
        );
    }
}