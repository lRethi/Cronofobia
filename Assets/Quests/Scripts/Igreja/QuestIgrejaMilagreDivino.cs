using Godot;

[GlobalClass]
public partial class QuestIgrejaMilagreDivino : QuestResource
{
    protected override void InicializarFlags()
    {
        Flags = new bool[1];
    }

    public override void Atualizar(GameState gameState)
    {
        if (!Ativa || Concluida)
            return;

        Flags[0] =
            gameState.GetFlag("igreja_frequencia_aceitou_interpretacao");

        if (!Flags[0])
            return;

        gameState.SetFlag(
            "igreja_assimilacao_desbloqueada",
            true
        );

        gameState.SetFlag(
            "final_assimilacao",
            true
        );

        gameState.SetFlag(
            "igreja_milagre_divino_concluida",
            true
        );

        Finalizar();
    }
}