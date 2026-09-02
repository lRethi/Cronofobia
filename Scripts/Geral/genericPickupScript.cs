using Godot;

public partial class genericPickupScript : Area3D
{
    [Export] public TipoPickup tipoPickup;
    [Export] public ItemDefinition item;
    [Export] public float valorRecurso = 1f;
    [Export] public float preco = 0f;
    [Export] public bool compravel = false;
    [Export] public PackedScene cenaCompra;

    public enum TipoPickup
    {
        Item,
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
            if (PegarItem())
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

        if (!PegarItem())
            return;

        NeedsState.Instance.SetDinheiro(
            NeedsState.Instance.varDinheiro - precoCompra
        );

        QueueFree();
    }

    private void RoubarItem()
    {
        if (!PegarItem())
            return;

        QueueFree();
    }

    private void FecharCompra()
    {
    }

    private bool PegarItem()
    {
        switch (tipoPickup)
        {
            case TipoPickup.Item:
                return InventoryState.Instance.AdicionarItem(item);

            case TipoPickup.Dinheiro:
                NeedsState.Instance.SetDinheiro(
                    NeedsState.Instance.varDinheiro + valorRecurso
                );

                return true;
        }

        return false;
    }
}