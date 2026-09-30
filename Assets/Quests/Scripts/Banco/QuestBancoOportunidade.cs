using Godot;

[GlobalClass]
public partial class QuestBancoOportunidade : QuestResource
{
    private const string ProximaQuestId = "banco_primeiro_dia";

    public QuestBancoOportunidade()
    {
        Id = "banco_oportunidade";
        Nome = "Banco — A Oportunidade";
        Descricao = "Uma oportunidade foi oferecida para você no banco. Talvez valha a pena checar.";
    }

    protected override void InicializarFlags()
    {
        Flags = new bool[2];
    }

    public override void Atualizar(GameState gameState)
    {
        if (!Ativa || Concluida)
            return;

        if (gameState.GetFlag("banco_oportunidadeAceita"))
        {
            Finalizar();
            gameState.StartQuest(ProximaQuestId);
            return;
        }

        if (gameState.GetFlag("banco_oportunidadeRecusada"))
            Finalizar();
    }
}