using Godot;

[GlobalClass]
public partial class QuestIgrejaPalavra : QuestResource
{
    private const string ProximaQuestId = "igreja_necessitados";

    private const int FlagAceitouAjuda = 0;
    private const int FlagTarefaConcluida = 1;

    protected override void InicializarFlags()
    {
        Flags = new bool[2];
    }

    public override void Atualizar(GameState gameState)
    {
        if (!Ativa || Concluida)
            return;

        Flags[FlagAceitouAjuda] =
            gameState.GetFlag("igreja_palavra_aceitou_ajuda");

        Flags[FlagTarefaConcluida] =
            gameState.GetFlag("igreja_palavra_tarefa_concluida");

        if (!Flags[FlagAceitouAjuda])
            return;

        if (!Flags[FlagTarefaConcluida])
            return;

        gameState.SetFlag(
            "igreja_palavra_concluida",
            true
        );

        Finalizar();

        gameState.StartQuest(ProximaQuestId);
    }
}