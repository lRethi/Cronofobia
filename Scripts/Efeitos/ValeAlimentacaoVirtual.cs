public partial class ValeAlimentacaoVirtual : EffectBase
{
    private bool usadoHoje;

    public override string Nome => "Vale-alimentação virtual";

    public override string Desc =>
        "Todo dia você pode comprar uma água ou um feijão de graça, mas marmitas e galões não podem mais ser encontrados.";

    public override string SpritePath =>
        "res://Assets/Sprites/Placeholder/the_placeholder.png";

    public override void aoEscolher()
    {
        GameState.Instance.SetFlag(
            "valeAlimentacaoVirtual",
            true
        );

        usadoHoje = false;
    }

    public override void InicioDoDia()
    {
        usadoHoje = false;
    }

    public override bool PodeComprarGratis(string itemId)
    {
        if (usadoHoje)
            return false;

        return itemId == EffectManager.IdGarrafaAgua ||
               itemId == EffectManager.IdLataFeijao;
    }

    public override void CompraConcluida(string itemId)
    {
        if (itemId == EffectManager.IdGarrafaAgua ||
            itemId == EffectManager.IdLataFeijao)
        {
            usadoHoje = true;
        }
    }
}