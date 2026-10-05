using Godot;

public partial class NeurochipDeCozinha : EffectBase
{
    public override string Nome => "Neurochip de Cozinha";

    public override string Desc =>
        "Permite fazer uma feijoada usando 2 feijões e 2 águas. A feijoada dá 3 de comida e 3 de água. Ao pegar um feijão, existe 50% de chance de ele ser imediatamente descartado.";

    public override string SpritePath =>
        "res://Assets/Sprites/Placeholder/the_placeholder.png";

    public override bool PermiteCozinharFeijoada()
    {
        return true;
    }

    public override bool DescartarFeijaoAoPegar(string itemId)
    {
        if (itemId != EffectManager.IdLataFeijao)
            return false;

        return GD.Randf() < 0.5f;
    }
}