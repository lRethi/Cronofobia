using Godot;

[GlobalClass]
public partial class QuestIgrejaNecessitados : QuestResource
{
    private const string ProximaQuestId = "igreja_milagre";

    private const int FlagComida = 0;
    private const int FlagAgua = 1;
    private const int FlagCobertor = 2;

    protected override void InicializarFlags()
    {
        Flags = new bool[3];
    }

    public override void Atualizar(GameState gameState)
    {
        if (!Ativa || Concluida)
            return;

        Flags[FlagComida] =
            gameState.GetFlag("igreja_necessitados_entrega_comida");

        Flags[FlagAgua] =
            gameState.GetFlag("igreja_necessitados_entrega_agua");

        Flags[FlagCobertor] =
            gameState.GetFlag("igreja_necessitados_entrega_cobertor");

        if (!Flags[FlagComida] ||
            !Flags[FlagAgua] ||
            !Flags[FlagCobertor])
            return;

        gameState.SetFlag(
            "igreja_necessitados_concluida",
            true
        );

        Finalizar();

        gameState.StartQuest(ProximaQuestId);
    }
}