using Godot;

public abstract partial class QuestResource : Resource
{
    [Export]
    public string Id { get; set; }

    [Export]
    public bool Ativa { get; set; }

    [Export]
    public bool Concluida { get; set; }

    public bool[] Flags { get; protected set; }

    public abstract void Atualizar(GameState gameState);

    protected abstract void InicializarFlags();

    public virtual void Iniciar()
    {
        Ativa = true;
        InicializarFlags();
    }

    public virtual void Finalizar()
    {
        Ativa = false;
        Concluida = true;
    }
}