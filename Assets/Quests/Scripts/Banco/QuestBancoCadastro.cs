using Godot;

[GlobalClass]
public partial class QuestBancoCadastro : QuestResource
{
    private const string ProximaQuestId = "banco_perfil_credito";

    protected override void InicializarFlags()
    {
        Flags = new bool[2];
    }

    public override void Atualizar(GameState gameState)
    {
        if (!Ativa || Concluida)
            return;

        Flags[0] =
            gameState.GetFlag("banco_cadastroFeito");

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