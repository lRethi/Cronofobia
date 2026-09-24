using Godot;

[GlobalClass]
public partial class QuestSemTetosTerreno : QuestResource
{
    private const string ProximaQuestId = "semtetos_inauguracao";

    private const int FlagEmpresa = 0;
    private const int FlagIgreja = 1;
    private const int FlagBanco = 2;
    private const int FlagAutorizacao = 3;
    private const int FlagIgnorado = 4;
    private const int FlagRecusado = 5;

    private const int DiasParaAutorizacao = 2;
    private const int PrimeiroDia = 1;
    private const int UltimoDia = 7;

    protected override void InicializarFlags()
    {
        Flags = new bool[6];
    }

    public override void Iniciar()
    {
        base.Iniciar();

        MarcarDiaInicio();
    }

    public override void Atualizar(GameState gameState)
    {
        if (!Ativa || Concluida)
            return;

        Flags[FlagEmpresa] = gameState.GetFlag("semtetos_buscar_empresa");
        Flags[FlagIgreja] = gameState.GetFlag("semtetos_desbloqueou_2_1");
        Flags[FlagBanco] = gameState.GetFlag("semtetos_desbloqueou_3_1");
        Flags[FlagAutorizacao] = gameState.GetFlag("semtetos_autorizacao_concedida");
        Flags[FlagIgnorado] = gameState.GetFlag("semtetos_terreno_ignorado");
        Flags[FlagRecusado] = gameState.GetFlag("semtetos_empresa_recusou");

        if (Flags[FlagIgnorado])
        {
            IniciarResultadoFalha(
                gameState,
                "semtetos_motivo_falha_ignorado"
            );

            return;
        }

        if (Flags[FlagRecusado])
        {
            IniciarResultadoFalha(
                gameState,
                "semtetos_motivo_falha_empresa_recusou"
            );

            return;
        }

        if (Flags[FlagAutorizacao])
        {
            DefinirCaminhoDeSucesso(gameState);

            gameState.SetFlag(
                "semtetos_resultado_13_sucesso",
                true
            );

            gameState.SetFlag(
                "semtetos_resultado_13_falha",
                false
            );

            Finalizar();

            gameState.StartQuest(ProximaQuestId);

            return;
        }

        if (PrazoExpirou(gameState))
        {
            IniciarResultadoFalha(
                gameState,
                "semtetos_motivo_falha_prazo"
            );
        }
    }

    private void MarcarDiaInicio()
    {
        int dia = Mathf.Clamp(
            Mathf.RoundToInt(TimeState.Instance.diaAtual),
            PrimeiroDia,
            UltimoDia
        );

        for (int i = PrimeiroDia; i <= UltimoDia; i++)
        {
            GameState.Instance.SetFlag(
                $"semtetos_terreno_inicio_dia_{i}",
                i == dia
            );
        }
    }

    private bool PrazoExpirou(GameState gameState)
    {
        int diaAtual = Mathf.RoundToInt(TimeState.Instance.diaAtual);

        for (int diaInicio = PrimeiroDia; diaInicio <= UltimoDia; diaInicio++)
        {
            if (!gameState.GetFlag($"semtetos_terreno_inicio_dia_{diaInicio}"))
                continue;

            int diasDecorridos = diaAtual - diaInicio;

            if (diasDecorridos < 0)
                diasDecorridos += UltimoDia;

            return diasDecorridos >= DiasParaAutorizacao;
        }

        return false;
    }

    private void DefinirCaminhoDeSucesso(GameState gameState)
    {
        gameState.SetFlag("semtetos_caminho_igreja", false);
        gameState.SetFlag("semtetos_caminho_banco", false);
        gameState.SetFlag("semtetos_caminho_empresa", false);

        if (gameState.GetFlag("semtetos_igreja_ajudou"))
        {
            gameState.SetFlag("semtetos_caminho_igreja", true);
            return;
        }

        if (gameState.GetFlag("semtetos_banco_ajudou"))
        {
            gameState.SetFlag("semtetos_caminho_banco", true);
            return;
        }

        gameState.SetFlag("semtetos_caminho_empresa", true);
    }

    private void IniciarResultadoFalha(
        GameState gameState,
        string motivoFlag
    )
    {
        gameState.SetFlag(motivoFlag, true);

        gameState.SetFlag(
            "semtetos_resultado_13_falha",
            true
        );

        gameState.SetFlag(
            "semtetos_resultado_13_sucesso",
            false
        );

        Finalizar();

        gameState.StartQuest(ProximaQuestId);
    }
}