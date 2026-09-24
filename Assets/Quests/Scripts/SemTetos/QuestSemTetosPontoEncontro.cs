using Godot;

[GlobalClass]
public partial class QuestSemTetosPontoEncontro : QuestResource
{
    private const string ProximaQuestId = "semtetos_terreno";

    private const int FlagConheceuGrupo = 0;
    private const int FlagComida = 1;
    private const int FlagCobertores = 2;
    private const int FlagMadeira = 3;
    private const int FlagFerramentas = 4;

    protected override void InicializarFlags()
    {
        Flags = new bool[5];
    }

    public override void Atualizar(GameState gameState)
    {
        if (!Ativa || Concluida)
            return;

        Flags[FlagConheceuGrupo] = gameState.GetFlag("semtetos_ponto_conheceu_grupo");
        Flags[FlagComida] = gameState.GetFlag("semtetos_comida");
        Flags[FlagCobertores] = gameState.GetFlag("semtetos_cobertores");
        Flags[FlagMadeira] = gameState.GetFlag("semtetos_madeira");
        Flags[FlagFerramentas] = gameState.GetFlag("semtetos_ferramentas");

        if (!Flags[FlagConheceuGrupo])
            return;

        if (!Flags[FlagComida] ||
            !Flags[FlagCobertores] ||
            !Flags[FlagMadeira] ||
            !Flags[FlagFerramentas])
            return;

        gameState.SetFlag("semtetos_ponto_encontro_concluido", true);

        Finalizar();

        gameState.StartQuest(ProximaQuestId);
    }
}