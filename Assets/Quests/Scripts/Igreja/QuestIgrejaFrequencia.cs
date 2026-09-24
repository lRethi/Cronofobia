using Godot;

[GlobalClass]
public partial class QuestIgrejaFrequencia : QuestResource
{
    private const string ProximaQuestReligiosa = "igreja_milagre_divino";
    private const string ProximaQuestTecnica = "igreja_deus_maquina";

    protected override void InicializarFlags()
    {
        Flags = new bool[3];
    }

    public override void Atualizar(GameState gameState)
    {
        if (!Ativa || Concluida)
            return;

        Flags[0] =
            gameState.GetFlag("igreja_milagre_aceitou_tecnologia");

        Flags[1] =
            gameState.GetFlag("igreja_frequencia_aceitou_interpretacao");

        Flags[2] =
            gameState.GetFlag("igreja_frequencia_investigar");

        if (!Flags[0])
            return;

        if (Flags[1])
        {
            gameState.SetFlag(
                "igreja_frequencia_investigar",
                false
            );

            gameState.SetFlag(
                "igreja_frequencia_concluida",
                true
            );

            Finalizar();

            gameState.StartQuest(ProximaQuestReligiosa);

            return;
        }

        if (Flags[2])
        {
            gameState.SetFlag(
                "igreja_frequencia_aceitou_interpretacao",
                false
            );

            gameState.SetFlag(
                "igreja_frequencia_concluida",
                true
            );

            Finalizar();

            gameState.StartQuest(ProximaQuestTecnica);
        }
    }
}