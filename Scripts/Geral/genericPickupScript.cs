using Godot;

public partial class genericPickupScript : Area3D
{
    [Export] public TipoPickup tipoPickup;
    [Export] public ItemDefinition item;
    [Export] public float valorRecurso = 1f;
    [Export] public float preco = 0f;
    [Export] public bool compravel = false;
    [Export] public PackedScene cenaCompra;
    [Export] public bool canSetFlag = false;
    [Export] public bool canSetFlagOnSteal = true;
    [Export] public string flagToSetOnPickup;
    [Export] public string flagToSetOnSteal;

    public enum TipoPickup
    {
        Item,
        Dinheiro
    }

    public override void _Ready()
    {
        GD.Print("[Pickup] _Ready: ", Name);
        GD.Print("[Pickup] Tipo: ", tipoPickup);
        GD.Print("[Pickup] Item: ", item != null ? item.Nome : "NULL");
        GD.Print("[Pickup] Comprável: ", compravel);

        BodyEntered += OnBodyEntered;
    }

    private void OnBodyEntered(Node3D body)
    {
        GD.Print("[Pickup] BodyEntered: ", body.Name);

        if (body is not movimentoPerson)
        {
            GD.Print("[Pickup] Corpo não é movimentoPerson. Ignorando.");
            return;
        }

        GD.Print("[Pickup] Jogador detectado.");

        if (!compravel)
        {
            GD.Print("[Pickup] Pickup não comprável. Tentando pegar item.");

            if (PegarItem())
            {
                GD.Print("[Pickup] PegarItem retornou TRUE. Removendo pickup.");
                QueueFree();
            }
            else
            {
                GD.PrintErr("[Pickup] PegarItem retornou FALSE. Pickup permanece.");
            }

            return;
        }

        GD.Print("[Pickup] Pickup comprável. Abrindo tela de compra.");

        MostrarCenaCompra();
    }

    private void MostrarCenaCompra()
    {
        GD.Print("[Pickup] MostrarCenaCompra chamado.");

        if (cenaCompra == null)
        {
            GD.PrintErr("[Pickup] cenaCompra está NULL.");
            return;
        }

        var cena = cenaCompra.Instantiate<cenaCompra>();

        GetTree().CurrentScene.AddChild(cena);

        cena.SetupScene(preco);

        cena.Comprar += ComprarItem;
        cena.Roubar += RoubarItem;
        cena.Fechar += FecharCompra;

        TimeState.Instance.CongelarTempo();

        GD.Print("[Pickup] Tela de compra criada.");
    }

    private void ComprarItem(int precoCompra)
    {
        GD.Print("[Pickup] ComprarItem chamado.");
        GD.Print("[Pickup] Preço: ", precoCompra);
        GD.Print("[Pickup] Dinheiro atual: ", NeedsState.Instance.varDinheiro);

        if (NeedsState.Instance.varDinheiro < precoCompra)
        {
            GD.Print("[Pickup] Dinheiro insuficiente.");
            return;
        }

        GD.Print("[Pickup] Tentando adicionar item ao inventário.");

        if (!PegarItem())
        {
            GD.PrintErr("[Pickup] PegarItem falhou. Compra cancelada.");
            return;
        }

        GD.Print("[Pickup] Item adicionado. Descontando dinheiro.");

        NeedsState.Instance.SetDinheiro(
            NeedsState.Instance.varDinheiro - precoCompra
        );

        GD.Print("[Pickup] Compra concluída. Removendo pickup.");

        QueueFree();
    }

    private void RoubarItem()
    {
        GD.Print("[Pickup] RoubarItem chamado.");

        if (!PegarItem())
        {
            GD.PrintErr("[Pickup] Não foi possível roubar o item.");
            return;
        }

        GD.Print("[Pickup] Item roubado com sucesso. Removendo pickup.");
        if(canSetFlagOnSteal && !string.IsNullOrEmpty(flagToSetOnSteal))
        {
            GD.Print("[Pickup] Definindo flag: ", flagToSetOnSteal);
            GameState.Instance.SetFlag(flagToSetOnSteal, true);
        }

        QueueFree();
    }

    private void FecharCompra()
    {
        GD.Print("[Pickup] FecharCompra chamado.");
    }

    private bool PegarItem()
    {
        GD.Print("[Pickup] PegarItem chamado.");
        GD.Print("[Pickup] Tipo do pickup: ", tipoPickup);

        switch (tipoPickup)
        {
            case TipoPickup.Item:

                if (item == null)
                {
                    GD.PrintErr("[Pickup] ItemDefinition está NULL.");
                    return false;
                }

                GD.Print("[Pickup] Item a adicionar: ", item.Nome);
                GD.Print("[Pickup] ID: ", item.Id);

                if (InventoryState.Instance == null)
                {
                    GD.PrintErr("[Pickup] InventoryState.Instance está NULL.");
                    return false;
                }

                GD.Print(
                    "[Pickup] Quantidade atual no inventário: ",
                    InventoryState.Instance.QuantidadeItens
                );

                bool resultado = InventoryState.Instance.AdicionarItem(item);

                GD.Print(
                    "[Pickup] AdicionarItem retornou: ",
                    resultado
                );

                GD.Print(
                    "[Pickup] Quantidade após tentativa: ",
                    InventoryState.Instance.QuantidadeItens
                );

                if (resultado && canSetFlag && !string.IsNullOrEmpty(flagToSetOnPickup))
                {
                    GD.Print("[Pickup] Definindo flag: ", flagToSetOnPickup);
                    GameState.Instance.SetFlag(flagToSetOnPickup, true);
                }

                return resultado;

            case TipoPickup.Dinheiro:

                GD.Print("[Pickup] Adicionando dinheiro: ", valorRecurso);

                NeedsState.Instance.SetDinheiro(
                    NeedsState.Instance.varDinheiro + valorRecurso
                );

                GD.Print(
                    "[Pickup] Dinheiro após adicionar: ",
                    NeedsState.Instance.varDinheiro
                );

                return true;
        }

        GD.PrintErr("[Pickup] TipoPickup desconhecido.");

        return false;
    }
}