using Godot;

[GlobalClass]
public partial class QuestIgrejaMilagre : QuestResource
{
    private const string ProximaQuestId = "igreja_frequencia";

    protected override void InicializarFlags()
    {
        Flags = new bool[3];
    }

    public override void Atualizar(GameState gameState)
    {
        if (!Ativa || Concluida)
            return;

        Flags[0] =
            gameState.GetFlag("igreja_milagre_presenciou_ia");

        Flags[1] =
            gameState.GetFlag("igreja_milagre_aceitou_tecnologia");

        Flags[2] =
            gameState.GetFlag("igreja_milagre_rejeitou_tecnologia");

        if (!Flags[0])
            return;

        if (Flags[1])
        {
            gameState.SetFlag(
                "igreja_milagre_rejeitou_tecnologia",
                false
            );

            gameState.SetFlag(
                "igreja_milagre_concluida",
                true
            );

            Finalizar();

            gameState.StartQuest(ProximaQuestId);

            return;
        }

        if (Flags[2])
        {
            gameState.SetFlag(
                "igreja_milagre_aceitou_tecnologia",
                false
            );

            gameState.SetFlag(
                "igreja_milagre_concluida",
                true
            );

            Finalizar();
        }
    }
}