public partial class NeurochipDeCozinha : EffectBase
{
    public override string Nome => "Neurochip de Cozinha";

    public override string Desc =>
        "Permite fazer uma feijoada usando 2 feijões e 2 águas. Itens em lojas passam a custar o dobro do preço.";

    public override string SpritePath =>
        "res://Assets/Sprites/Placeholder/the_placeholder.png";

    public override float MultiplicadorPreco(string itemId)
    {
        return 2f;
    }

    public override bool PermiteCozinharFeijoada()
    {
        return true;
    }
}