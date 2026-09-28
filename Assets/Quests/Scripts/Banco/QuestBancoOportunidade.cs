using Godot;

[GlobalClass]
public partial class QuestBancoOportunidade : QuestResource
{
    private const string ProximaQuestId = "banco_primeiro_dia";

    protected override void InicializarFlags()
    {
        Flags = new bool[2];
    }

    public override void Atualizar(GameState gameState)
    {
        if (!Ativa || Concluida)
            return;

        Flags[0] =
            gameState.GetFlag("banco_oportunidadeAceita");

        Flags[1] =
            gameState.GetFlag("banco_oportunidadeRecusada");

        if (Flags[0])
        {
            Finalizar();
            gameState.StartQuest(ProximaQuestId);
            return;
        }

        if (Flags[1])
        {
            Finalizar();
        }
    }
}