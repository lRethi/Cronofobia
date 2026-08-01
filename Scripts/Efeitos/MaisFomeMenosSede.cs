using System;

public partial class MaisFomeMenosSede : EffectBase
{
    public override string Nome => "Mais Fome, Menos Sede";
    public override string Desc => "A IA passa a rapidamente metabolizar sua comida, mas a sede passa a ser menos intensa. Aumente a fome máxima em 1 e diminua a sede máxima em 1.";
    public override string SpritePath => "res://Assets/Sprites/Placeholder/the_placeholder.png";
    public override void aoEscolher()
    {
        NeedsState.Instance.AlterarMaximoFome(NeedsState.Instance.maximoFome + 1f);
        NeedsState.Instance.AlterarMaximoSede(NeedsState.Instance.maximoSede - 1f);
    }
}
