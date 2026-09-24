using Godot;

[GlobalClass]
public partial class QuestSemTetosInauguracao : QuestResource
{
    private const string ProximaQuestId = "semtetos_novo_lugar";

    private const int FlagSucesso = 0;
    private const int FlagFalha = 1;
    private const int FlagInauguracao = 2;
    private const int FlagExpulsao = 3;

    protected override void InicializarFlags()
    {
        Flags = new bool[4];
    }

    public override void Atualizar(GameState gameState)
    {
        if (!Ativa || Concluida)
            return;

        Flags[FlagSucesso] =
            gameState.GetFlag("semtetos_resultado_13_sucesso");

        Flags[FlagFalha] =
            gameState.GetFlag("semtetos_resultado_13_falha");

        Flags[FlagInauguracao] =
            gameState.GetFlag("semtetos_inauguracao_realizada");

        Flags[FlagExpulsao] =
            gameState.GetFlag("semtetos_expulsao_concluida");

        if (Flags[FlagSucesso] && Flags[FlagInauguracao])
        {
            gameState.SetFlag(
                "semtetos_abrigo_inconstante_desbloqueado",
                true
            );

            gameState.SetFlag(
                "final_abrigo_inconstante",
                true
            );

            gameState.SetFlag(
                "semtetos_abrigo_inaugurado",
                true
            );

            Finalizar();

            return;
        }

        if (Flags[FlagFalha] && Flags[FlagExpulsao])
        {
            gameState.SetFlag(
                "semtetos_grupo_precisa_sair",
                true
            );

            Finalizar();

            gameState.StartQuest(ProximaQuestId);
        }
    }
}