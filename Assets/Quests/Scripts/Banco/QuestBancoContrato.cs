using Godot;

[GlobalClass]
public partial class QuestBancoContrato : QuestResource
{
    protected override void InicializarFlags()
    {
        Flags = new bool[2];
    }

    public override void Atualizar(GameState gameState)
    {
        if (!Ativa || Concluida)
            return;

        Flags[0] =
            gameState.GetFlag("banco_oportunidadeFinalizada");

        Flags[1] =
            gameState.GetFlag("empregoEstavel");

        if (!Flags[0] || !Flags[1])
            return;

        gameState.SetFlag(
            "final_emprego_estavel",
            true
        );

        Finalizar();
    }
}