using Godot;

public partial class genericPickupScript : Area3D
{
    [Export] public TipoRecursoEnum tipoRecurso;
    [Export] public float valorRecurso = 1f;
    [Export] public float preco = 0f;
    [Export] public bool compravel = false;
    [Export] public PackedScene cenaCompra;

    public enum TipoRecursoEnum
    {
        Fome,
        Sede,
        Dinheiro
    }

    public override void _Ready()
    {
        BodyEntered += OnBodyEntered;
    }

    private void OnBodyEntered(Node3D body)
    {
        if (body is not movimentoPerson)
            return;

        if (!compravel)
        {
            PegarItem();
            QueueFree();
            return;
        }

        MostrarCenaCompra();
    }

    private void MostrarCenaCompra()
    {
        var cena = cenaCompra.Instantiate<cenaCompra>();

        GetTree().CurrentScene.AddChild(cena);

        cena.SetupScene(preco);

        cena.Comprar += ComprarItem;
        cena.Roubar += RoubarItem;
        cena.Fechar += FecharCompra;
        TimeState.Instance.CongelarTempo();
    }

    private void ComprarItem(int precoCompra)
    {
        if (NeedsState.Instance.varDinheiro < precoCompra)
            return;

        NeedsState.Instance.SetDinheiro(
            NeedsState.Instance.varDinheiro - precoCompra
        );

        PegarItem();
        QueueFree();
    }

    private void RoubarItem()
    {
        PegarItem();
        QueueFree();
    }

    private void FecharCompra()
    {
    }

    private void PegarItem()
    {
        switch (tipoRecurso)
        {
            case TipoRecursoEnum.Fome:
                NeedsState.Instance.SetFome(
                    NeedsState.Instance.varFome + valorRecurso
                );
                break;

            case TipoRecursoEnum.Sede:
                NeedsState.Instance.SetSede(
                    NeedsState.Instance.varSede + valorRecurso
                );
                break;

            case TipoRecursoEnum.Dinheiro:
                NeedsState.Instance.SetDinheiro(
                    NeedsState.Instance.varDinheiro + valorRecurso
                );
                break;
        }
    }
}