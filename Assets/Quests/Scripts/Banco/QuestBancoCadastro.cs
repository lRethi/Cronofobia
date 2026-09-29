using Godot;

[GlobalClass]
public partial class QuestBancoCadastro : QuestResource
{
    private const string ProximaQuestId = "banco_oportunidade";

    protected override void InicializarFlags()
    {
        Flags = new bool[2];
    }

    public QuestBancoCadastro()
    {
        Id = "banco_cadastro";
        Nome = "Banco — O Cadastro";
        Descricao = "Regularize seus dados para poder usar os serviços do banco.";
    }

    public override void Atualizar(GameState gameState)
    {
        if (!Ativa || Concluida)
            return;

        if (gameState.GetFlag("banco_cadastroFeito"))
        {
            Finalizar();
            gameState.StartQuest(ProximaQuestId);
            return;
        }

        if (gameState.GetFlag("perfil_incompativel"))
            Finalizar();
    }
}