public partial class TransacaoAutomatica : EffectBase
{
    public override string Nome => "Transação Automática";

    public override string Desc =>
        "Ao escolher uma opção de diálogo com alguma pessoa, ganhe 1 moeda, mas não encontre mais moedas na rua.";

    public override string SpritePath =>
        "res://Assets/Sprites/Placeholder/the_placeholder.png";

    public override void aoEscolher()
    {
        GameState.Instance.SetFlag(
            Nome,
            true
        );
    }

    public override void AoEscolherOpcaoDialogo()
    {
        NeedsState.Instance.SetDinheiro(
            NeedsState.Instance.varDinheiro + 1f
        );
    }
}