using Godot;

[GlobalClass]
public partial class QuestAguaMinhoca : QuestResource
{
    public const int AceitouAjuda = 0;
    public const int RecusouAjuda = 1;
    public const int MinhocaFinalizado = 2;

    protected override void InicializarFlags()
    {
        Flags = new bool[3];
    }

    public override void Atualizar(GameState gameState)
    {
        if (!Ativa || Concluida)
            return;

        Flags[AceitouAjuda] = gameState.GetFlag("aceitou_ajuda");
        Flags[RecusouAjuda] = gameState.GetFlag("recusou_ajuda");

        if (gameState.GetFlag("minhoca_finalizado"))
            Flags[MinhocaFinalizado] = true;

        if (Flags[MinhocaFinalizado])
            Finalizar();
    }
}