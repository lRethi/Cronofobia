using Godot;

[GlobalClass]
public partial class QuestBancoPrimeiroDia : QuestResource
{
    private const string ProximaQuestId = "banco_contrato";

    protected override void InicializarFlags()
    {
        Flags = new bool[4];
    }

    public override void Atualizar(GameState gameState)
    {
        if (!Ativa || Concluida)
            return;

        Flags[0] =
            gameState.GetFlag("banco_cliente1");

        Flags[1] =
            gameState.GetFlag("banco_cliente2");

        Flags[2] =
            gameState.GetFlag("banco_cliente3");

        Flags[3] =
            gameState.GetFlag("banco_cliente4");

        if (!Flags[0] ||
            !Flags[1] ||
            !Flags[2] ||
            !Flags[3])
            return;

        Finalizar();
        gameState.StartQuest(ProximaQuestId);
    }
}