using Godot;

[GlobalClass]
public partial class QuestBancoPerfilCredito : QuestResource
{
    private const string ProximaQuestId = "banco_oportunidade";

    protected override void InicializarFlags()
    {
        Flags = new bool[2];
    }

    public override void Atualizar(GameState gameState)
    {
        if (!Ativa || Concluida)
            return;

        Flags[0] =
            gameState.GetFlag("banco_perfilDeCreditoMelhorado");

        Flags[1] =
            gameState.GetFlag("perfil_incompativel");

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