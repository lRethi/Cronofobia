using Godot;

[GlobalClass]
public partial class QuestBancoContrato : QuestResource
{
    public QuestBancoContrato()
    {
        Id = "banco_contrato";
        Nome = "Banco — O Contrato";
        Descricao = "Você fez o seu melhor, agora é hora de ver se conseguiu o emprego ou não.";
    }

    protected override void InicializarFlags()
    {
        Flags = new bool[2];
    }

    public override void Atualizar(GameState gameState)
    {
        if (!Ativa || Concluida)
            return;

        if (!gameState.GetFlag("banco_oportunidadeFinalizada"))
            return;

        if (gameState.GetFlag("empregoEstavel"))
                gameState.SetFlag("final_emprego_estavel", true);

        Finalizar();
    }
}