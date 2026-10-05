public partial class NovosImpostos : EffectBase
{
    private bool descontoAtivo;

    public override string Nome => "Novos Impostos";

    public override string Desc =>
        "Você perde 5 moedas após dormir, mas todas as comidas custam 50% menos. Se não tiver 5 moedas, o efeito não funciona.";

    public override string SpritePath =>
        "res://Assets/Sprites/Placeholder/the_placeholder.png";

    public override void InicioDoDia()
    {
        descontoAtivo = false;
    }

    public override void FimDoDia()
    {
        descontoAtivo = false;

        if (NeedsState.Instance.varDinheiro < 5f)
            return;

        NeedsState.Instance.SetDinheiro(
            NeedsState.Instance.varDinheiro - 5f
        );

        descontoAtivo = true;
    }

    public override float MultiplicadorPreco(string itemId)
    {
        if (!descontoAtivo)
            return 1f;

        if (itemId == EffectManager.IdLataFeijao ||
            itemId == EffectManager.IdMarmitta ||
            itemId == EffectManager.IdFeijoada)
        {
            return 0.5f;
        }

        return 1f;
    }
}