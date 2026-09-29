using Godot;

[GlobalClass]
public partial class QuestBancoPrimeiroDia : QuestResource
{
    private const string ProximaQuestId = "banco_contrato";

    public QuestBancoPrimeiroDia()
    {
        Id = "banco_primeiro_dia";
        Nome = "Banco — Primeiro Dia";
        Descricao = "Atenda os clientes no banco para conseguir um emprego";
    }

    protected override void InicializarFlags()
    {
        Flags = new bool[4];
    }

    public override string GetDescricao()
    {
        int clientesAtendidos = 0;

        if (Flags[0])
            clientesAtendidos++;

        if (Flags[1])
            clientesAtendidos++;

        if (Flags[2])
            clientesAtendidos++;

        if (Flags[3])
            clientesAtendidos++;

        return $"Clientes atendidos {clientesAtendidos}/4";
    }

    public override void Atualizar(GameState gameState)
    {
        if (!Ativa || Concluida)
            return;

        Flags[0] = gameState.GetFlag("banco_cliente1");
        Flags[1] = gameState.GetFlag("banco_cliente2");
        Flags[2] = gameState.GetFlag("banco_cliente3");
        Flags[3] = gameState.GetFlag("banco_cliente4");

        if (!Flags[0] || !Flags[1] || !Flags[2] || !Flags[3])
            return;

        Finalizar();
        gameState.StartQuest(ProximaQuestId);
    }
}