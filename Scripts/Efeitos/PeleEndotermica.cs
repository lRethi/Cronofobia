public partial class PeleEndotermica : EffectBase
{
    public override string Nome => "Pele Endotérmica";

    public override string Desc =>
        "Permite dormir em qualquer lugar no fim do dia, mas ocupa 1 espaço no seu inventário.";

    public override string SpritePath =>
        "res://Assets/Sprites/Placeholder/the_placeholder.png";

    public override bool PodeEscolher()
    {
        if (InventoryState.Instance == null)
            return false;

        if (EffectManager.Instance == null)
            return false;

        ItemDefinition item = EffectManager.Instance.ObterItemPorId(
            EffectManager.IdBlockInventario
        );

        if (item == null)
            return false;

        return !InventoryState.Instance.EstaCheio();
    }

    public override void aoEscolher()
    {
        if (InventoryState.Instance == null)
            return;

        if (EffectManager.Instance == null)
            return;

        ItemDefinition item = EffectManager.Instance.ObterItemPorId(
            EffectManager.IdBlockInventario
        );

        if (item == null)
            return;

        InventoryState.Instance.AdicionarItem(item);
    }

    public override bool PermiteDormirSemLugar()
    {
        return true;
    }
}