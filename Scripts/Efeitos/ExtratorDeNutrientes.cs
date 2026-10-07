using Godot;

public partial class ExtratorDeNutrientes : EffectBase
{
    public override string Nome => "Extrator de Nutrientes";

    public override string Desc =>
        "Permite que feijões preencham 1 de fome e 1 de sede ao serem consumidos. Ao pegar um feijão, existe 50% de chance de ele ser imediatamente descartado.";

    public override string SpritePath =>
        "res://Assets/Sprites/Placeholder/the_placeholder.png";

    public override float ModificarFomeAoUsar(string itemId, float valor)
    {
        if (itemId != EffectManager.IdLataFeijao)
            return valor;

        return 1f;
    }

    public override float ModificarSedeAoUsar(string itemId, float valor)
    {
        if (itemId != EffectManager.IdLataFeijao)
            return valor;

        return 1f;
    }

    public override bool DescartarFeijaoAoPegar(string itemId)
    {
        if (itemId != EffectManager.IdLataFeijao)
            return false;

        return GD.Randf() < 0.5f;
    }
}