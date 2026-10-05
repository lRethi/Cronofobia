public partial class TransformadorBionico : EffectBase
{
    public override string Nome => "Transformador Biônico";

    public override string Desc =>
        "Itens em lojas custam o dobro, mas água e comida podem ser transformadas entre si.";

    public override string SpritePath =>
        "res://Assets/Sprites/Placeholder/the_placeholder.png";

    public override float MultiplicadorPreco(string itemId)
    {
        return 2f;
    }

    public override bool PermiteTransformarAguaComida()
    {
        return true;
    }
}