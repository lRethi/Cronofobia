using Godot;

public partial class MochilaModerna : EffectBase
{
    public override string Nome => "Mochila Moderna";

    public override string Desc =>
        "Ganhe mais 2 de espaço, mas ande 5% mais devagar por item carregado, até 30% de penalidade.";

    public override string SpritePath =>
        "res://Assets/Sprites/Placeholder/the_placeholder.png";

    public override int BonusSlotsInventario()
    {
        return 2;
    }

    public override float MultiplicadorVelocidade()
    {
        if (InventoryState.Instance == null)
            return 1f;

        int quantidade =
            InventoryState.Instance.QuantidadeItens;

        float penalidade =
            Mathf.Min(
                quantidade * 0.05f,
                0.30f
            );

        return 1f - penalidade;
    }
}