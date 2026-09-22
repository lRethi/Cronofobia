using Godot;
using System.Collections.Generic;
using System.Threading.Tasks;

public partial class InventoryState : Node
{
    public static InventoryState Instance { get; private set; }

    [Export]
    public string ItemOferecidoId { get; set; } = "";

    private int quantidadeSlots = 4;
    private List<ItemDefinition> itensInventario = new List<ItemDefinition>();
    private ItemDefinition itemOferecido;

    public int QuantidadeSlots => quantidadeSlots;

    public IReadOnlyList<ItemDefinition> ItensInventario => itensInventario;

    public int QuantidadeItens => itensInventario.Count;

    [Signal]
    public delegate void InventarioAlteradoEventHandler();

    [Signal]
    public delegate void OfertaSolicitadaEventHandler();

    [Signal]
    public delegate void OfertaFinalizadaEventHandler();

    public override void _Ready()
    {
        GD.Print("[InventoryState] _Ready iniciado.");

        if (Instance != null && Instance != this)
        {
            GD.PrintErr("[InventoryState] Já existe outra Instance. Liberando esta.");
            QueueFree();
            return;
        }

        Instance = this;

        GD.Print("[InventoryState] Instance definida.");
        GD.Print("[InventoryState] Quantidade de slots: ", quantidadeSlots);
        GD.Print("[InventoryState] Itens atuais: ", itensInventario.Count);
    }

    public bool UsarItem(int index)
    {
        GD.Print("[InventoryState] UsarItem chamado. Index: ", index);

        if (index < 0 || index >= itensInventario.Count)
        {
            GD.PrintErr("[InventoryState] UsarItem falhou: index inválido.");
            return false;
        }

        ItemDefinition item = itensInventario[index];

        if (item == null)
        {
            GD.PrintErr("[InventoryState] UsarItem falhou: item é null.");
            return false;
        }

        GD.Print("[InventoryState] Usando item: ", item.Nome);

        if (!item.PodeSerUsado)
        {
            GD.Print("[InventoryState] Item não pode ser usado.");
            return false;
        }

        NeedsState.Instance.SetFome(
            NeedsState.Instance.varFome + item.FomeAoUsar
        );

        NeedsState.Instance.SetSede(
            NeedsState.Instance.varSede + item.SedeAoUsar
        );

        return RemoverItem(index);
    }

    public bool AdicionarItem(ItemDefinition item)
    {
        GD.Print("[InventoryState] AdicionarItem chamado.");

        if (item == null)
        {
            GD.PrintErr("[InventoryState] AdicionarItem falhou: ItemDefinition é NULL.");
            return false;
        }

        GD.Print("[InventoryState] Item recebido: ", item.Nome);
        GD.Print("[InventoryState] ID do item: ", item.Id);
        GD.Print("[InventoryState] Itens antes de adicionar: ", itensInventario.Count);
        GD.Print("[InventoryState] Slots máximos: ", quantidadeSlots);

        if (EstaCheio())
        {
            GD.PrintErr("[InventoryState] AdicionarItem falhou: inventário cheio.");
            return false;
        }

        itensInventario.Add(item);

        GD.Print("[InventoryState] Item adicionado com sucesso.");
        GD.Print("[InventoryState] Itens depois de adicionar: ", itensInventario.Count);
        GD.Print("[InventoryState] Emitindo InventarioAlterado.");

        EmitSignal(SignalName.InventarioAlterado);

        GD.Print("[InventoryState] InventarioAlterado emitido.");

        return true;
    }

    public bool RemoverItem(int index)
    {
        GD.Print("[InventoryState] RemoverItem chamado. Index: ", index);

        if (index < 0 || index >= itensInventario.Count)
        {
            GD.PrintErr("[InventoryState] RemoverItem falhou: index inválido.");
            return false;
        }

        ItemDefinition item = itensInventario[index];

        GD.Print(
            "[InventoryState] Removendo item: ",
            item != null ? item.Nome : "NULL"
        );

        itensInventario.RemoveAt(index);

        GD.Print("[InventoryState] Itens restantes: ", itensInventario.Count);
        GD.Print("[InventoryState] Emitindo InventarioAlterado.");

        EmitSignal(SignalName.InventarioAlterado);

        return true;
    }

    public bool PossuiItem(ItemDefinition item)
    {
        bool possui = item != null && itensInventario.Contains(item);

        GD.Print(
            "[InventoryState] PossuiItem: ",
            item != null ? item.Nome : "NULL",
            " -> ",
            possui
        );

        return possui;
    }

    public int EncontrarItem(ItemDefinition item)
    {
        int index = itensInventario.IndexOf(item);

        GD.Print(
            "[InventoryState] EncontrarItem: ",
            item != null ? item.Nome : "NULL",
            " -> index ",
            index
        );

        return index;
    }

    public bool EstaCheio()
    {
        bool cheio = itensInventario.Count >= quantidadeSlots;

        GD.Print(
            "[InventoryState] EstaCheio: ",
            cheio,
            " | ",
            itensInventario.Count,
            "/",
            quantidadeSlots
        );

        return cheio;
    }

    public void Limpar()
    {
        GD.Print("[InventoryState] Limpando inventário.");

        itensInventario.Clear();
        itemOferecido = null;
        ItemOferecidoId = "";

        GD.Print("[InventoryState] Inventário limpo.");
        EmitSignal(SignalName.InventarioAlterado);
    }

    public async Task OferecerItem()
    {
        GD.Print("[InventoryState] OferecerItem iniciado.");

        itemOferecido = null;
        ItemOferecidoId = "";

        GD.Print("[InventoryState] Emitindo OfertaSolicitada.");

        EmitSignal(SignalName.OfertaSolicitada);

        await ToSignal(this, SignalName.OfertaFinalizada);

        GD.Print("[InventoryState] OfertaFinalizada recebida.");
    }

    public bool PodeOferecerItem(ItemDefinition item)
    {
        bool pode = item != null && item.PodeSerDescartado;

        GD.Print(
            "[InventoryState] PodeOferecerItem: ",
            item != null ? item.Nome : "NULL",
            " -> ",
            pode
        );

        return pode;
    }

    public bool ConfirmarOferta(ItemDefinition item)
    {
        GD.Print(
            "[InventoryState] ConfirmarOferta chamado: ",
            item != null ? item.Nome : "NULL"
        );

        if (!PodeOferecerItem(item))
        {
            GD.PrintErr("[InventoryState] ConfirmarOferta falhou: item não pode ser oferecido.");
            return false;
        }

        if (!PossuiItem(item))
        {
            GD.PrintErr("[InventoryState] ConfirmarOferta falhou: item não está no inventário.");
            return false;
        }

        itemOferecido = item;
        ItemOferecidoId = item.Id;

        GD.Print("[InventoryState] Oferta confirmada.");
        GD.Print("[InventoryState] ItemOferecidoId: ", ItemOferecidoId);

        EmitSignal(SignalName.OfertaFinalizada);

        return true;
    }

    public void CancelarOferta()
    {
        GD.Print("[InventoryState] Oferta cancelada.");

        itemOferecido = null;
        ItemOferecidoId = "";

        EmitSignal(SignalName.OfertaFinalizada);
    }

    public bool ConsumirItemOferecido()
    {
        GD.Print("[InventoryState] ConsumirItemOferecido chamado.");

        if (itemOferecido == null)
        {
            GD.PrintErr("[InventoryState] Não existe item oferecido.");
            return false;
        }

        GD.Print("[InventoryState] Item oferecido: ", itemOferecido.Nome);

        int index = EncontrarItem(itemOferecido);

        if (index < 0)
        {
            GD.PrintErr("[InventoryState] Item oferecido não encontrado no inventário.");

            itemOferecido = null;
            ItemOferecidoId = "";

            return false;
        }

        bool removido = RemoverItem(index);

        GD.Print("[InventoryState] Item oferecido removido: ", removido);

        if (removido)
        {
            itemOferecido = null;
            ItemOferecidoId = "";
        }

        return removido;
    }
}