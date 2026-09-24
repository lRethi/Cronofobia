using Godot;

[GlobalClass]
public partial class QuestSemTetosNovoLugar : QuestResource
{
    private const int FlagBusca = 0;
    private const int FlagNovoLugar = 1;
    private const int FlagAbandonou = 2;

    protected override void InicializarFlags()
    {
        Flags = new bool[3];
    }

    public override void Atualizar(GameState gameState)
    {
        if (!Ativa || Concluida)
            return;

        Flags[FlagBusca] =
            gameState.GetFlag("semtetos_procurar_novo_lugar");

        Flags[FlagNovoLugar] =
            gameState.GetFlag("semtetos_novo_lugar_encontrado");

        Flags[FlagAbandonou] =
            gameState.GetFlag("semtetos_grupo_abandonado");

        if (Flags[FlagAbandonou])
        {
            gameState.SetFlag(
                "semtetos_grupo_acesso",
                false
            );

            gameState.SetFlag(
                "semtetos_abrigo_improvisado",
                false
            );

            Finalizar();

            return;
        }

        if (Flags[FlagNovoLugar])
        {
            gameState.SetFlag(
                "semtetos_grupo_acesso",
                true
            );

            gameState.SetFlag(
                "semtetos_abrigo_improvisado",
                true
            );

            gameState.SetFlag(
                "semtetos_abrigo_inconstante_desbloqueado",
                false
            );

            gameState.SetFlag(
                "final_abrigo_inconstante",
                false
            );

            Finalizar();
        }
    }
}