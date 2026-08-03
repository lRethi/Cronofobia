public partial class DinheiroEmComida : EffectBase
{
    public override string Nome => "Dinheiro em Comida";
    public override string Desc => "Como um passe de mágica, seu corpo passa a se mover durante o seu sono, automaticamente consumindo seu dinheiro para comprar comida. Inicie todo dia com 1 de comida, mas com 5 de dinheiro a menos.";
    public override string SpritePath => "res://Assets/Sprites/Placeholder/the_placeholder.png";
    public override void aoEscolher()
    {
        
    }
    public override void InicioDoDia()
    {
        if(NeedsState.Instance.varDinheiro >= 5f)
        {
            NeedsState.Instance.SetFome(NeedsState.Instance.varFome + 1f);
            NeedsState.Instance.SetDinheiro(NeedsState.Instance.varDinheiro - 5f);
        }
    }
}