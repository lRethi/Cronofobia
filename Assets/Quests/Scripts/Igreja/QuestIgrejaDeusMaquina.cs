using Godot;

[GlobalClass]
public partial class QuestIgrejaDeusMaquina : QuestResource
{
    protected override void InicializarFlags()
    {
        Flags = new bool[3];
    }

    public override void Atualizar(GameState gameState)
    {
        if (!Ativa || Concluida)
            return;

        Flags[0] =
            gameState.GetFlag("igreja_deus_maquina_causa_descoberta");

        Flags[1] =
            gameState.GetFlag("igreja_deus_maquina_incentivou_culto");

        Flags[2] =
            gameState.GetFlag("igreja_deus_maquina_rejeitou_culto");

        if (!Flags[0])
            return;

        if (Flags[1])
        {
            gameState.SetFlag(
                "igreja_deus_maquina_rejeitou_culto",
                false
            );

            gameState.SetFlag(
                "igreja_deus_maquina_desbloqueado",
                true
            );

            gameState.SetFlag(
                "final_deus_maquina",
                true
            );

            gameState.SetFlag(
                "igreja_deus_maquina_concluida",
                true
            );

            Finalizar();

            return;
        }

        if (Flags[2])
        {
            gameState.SetFlag(
                "igreja_deus_maquina_incentivou_culto",
                false
            );

            gameState.SetFlag(
                "igreja_deus_maquina_concluida",
                true
            );

            Finalizar();
        }
    }
}