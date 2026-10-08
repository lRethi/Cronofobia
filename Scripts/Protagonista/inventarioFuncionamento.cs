using Godot;

public partial class inventarioFuncionamento : Control
{
    [Export] public Button[] slotsInventario;
    [Export] public TextureRect[] texturasSlots;
    [Export] public Panel telaUso;
    [Export] public Label lblNomeItem;
    [Export] public Label lblDescricaoItem;
    [Export] public Button botUsar;
    [Export] public Button botDescartar;
    [Export] public Button botSair;
    [Export] public Panel telaOferecer;
    [Export] public Button botSim;
    [Export] public Button botNao;

    private int slotSelecionado = -1;
    private bool modoOferecer = false;

    public override void _Ready()
    {
        texturasSlots = new TextureRect[]
        {
            GetNode<TextureRect>("Base/Slot1/TextureRect"),
            GetNode<TextureRect>("Base/Slot2/TextureRect2"),
            GetNode<TextureRect>("Base/Slot3/TextureRect3"),
            GetNode<TextureRect>("Base/Slot4/TextureRect4"),
            GetNode<TextureRect>("Base/Base2/Slot1/TextureRect"),
            GetNode<TextureRect>("Base/Base2/Slot2/TextureRect2")
        };

        slotsInventario = new Button[]
        {
            GetNode<Button>("Base/Slot1"),
            GetNode<Button>("Base/Slot2"),
            GetNode<Button>("Base/Slot3"),
            GetNode<Button>("Base/Slot4"),
            GetNode<Button>("Base/Base2/Slot1"),
            GetNode<Button>("Base/Base2/Slot2")
        };

        GD.Print("[InventarioUI] _Ready iniciado.");

        for (int i = 0; i < slotsInventario.Length; i++)
        {
            int index = i;
            slotsInventario[i].Pressed += () => SelecionarSlot(index);
            slotsInventario[i].FocusMode = Control.FocusModeEnum.None;
        }

        botUsar.Pressed += UsarItem;
        botDescartar.Pressed += DescartarItem;
        botSair.Pressed += FecharTelaUso;
        botSim.Pressed += ConfirmarOferta;
        botNao.Pressed += CancelarOferta;

        InventoryState.Instance.OfertaSolicitada += AbrirParaOferecer;

        botUsar.FocusMode = Control.FocusModeEnum.None;
        botDescartar.FocusMode = Control.FocusModeEnum.None;
        botSair.FocusMode = Control.FocusModeEnum.None;
        botSim.FocusMode = Control.FocusModeEnum.None;
        botNao.FocusMode = Control.FocusModeEnum.None;

        telaUso.Visible = false;
        telaOferecer.Visible = false;
        Visible = false;

        InventoryState.Instance.InventarioAlterado += AtualizarInventario;

        GD.Print("[InventarioUI] Signal InventarioAlterado conectado.");

        AtualizarInventario();

        GD.Print("[InventarioUI] _Ready concluído.");
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        if (!@event.IsActionPressed("open_inventory"))
            return;

        GD.Print("[InventarioUI] open_inventory pressionado.");

        if (GameState.Instance == null)
            return;

        if (GameState.Instance.DialogoAberto)
            return;

        AlternarInventario();

        GetViewport().SetInputAsHandled();
    }

    private void AlternarInventario()
    {
        Visible = !Visible;

        GD.Print("[InventarioUI] Inventário visível: ", Visible);

        if (Visible)
        {
            TimeState.Instance.CongelarTempo();
            GameState.Instance.SetCameraInputEnabled(false);
            GameState.Instance.SetCameraMouseCaptured(false);
        }
        else
        {
            telaUso.Visible = false;
            telaOferecer.Visible = false;
            slotSelecionado = -1;
            modoOferecer = false;

            TimeState.Instance.DescongelarTempo();
            GameState.Instance.SetCameraInputEnabled(true);
            GameState.Instance.SetCameraMouseCaptured(true);
        }
    }

    public void AbrirParaOferecer()
    {
        GD.Print("[InventarioUI] AbrirParaOferecer chamado.");

        modoOferecer = true;
        slotSelecionado = -1;

        telaUso.Hide();
        telaOferecer.Hide();

        Show();

        GameState.Instance.SetCameraInputEnabled(false);
        GameState.Instance.SetCameraMouseCaptured(false);
    }

    private void SelecionarSlot(int index)
    {
        GD.Print("[InventarioUI] SelecionarSlot: ", index);

        if (index < 0 || index >= InventoryState.Instance.QuantidadeItens)
        {
            GD.PrintErr("[InventarioUI] Index de slot inválido.");
            return;
        }

        var itens = InventoryState.Instance.ItensInventario;
        var item = itens[index];

        GD.Print(
            "[InventarioUI] Item selecionado: ",
            item != null ? item.Nome : "NULL"
        );

        if (modoOferecer)
        {
            if (!InventoryState.Instance.PodeOferecerItem(item))
                return;

            slotSelecionado = index;
            telaOferecer.Show();

            return;
        }

        slotSelecionado = index;

        lblNomeItem.Text = item.Nome;
        lblDescricaoItem.Text = item.Descricao;

        botUsar.Visible = item.PodeSerUsado;
        botDescartar.Visible = item.PodeSerDescartado;

        telaUso.Show();
    }

    private void UsarItem()
    {
        GD.Print("[InventarioUI] UsarItem. Slot: ", slotSelecionado);

        if (slotSelecionado < 0)
            return;

        if (InventoryState.Instance.UsarItem(slotSelecionado))
            FecharTelaUso();
    }

    private void DescartarItem()
    {
        GD.Print("[InventarioUI] DescartarItem. Slot: ", slotSelecionado);

        if (slotSelecionado < 0)
            return;

        if (InventoryState.Instance.RemoverItem(slotSelecionado))
            FecharTelaUso();
    }

    private void FecharTelaUso()
    {
        GD.Print("[InventarioUI] FecharTelaUso.");

        telaUso.Visible = false;
        slotSelecionado = -1;
    }

    private void ConfirmarOferta()
    {
        GD.Print("[InventarioUI] ConfirmarOferta. Slot: ", slotSelecionado);

        if (slotSelecionado < 0)
            return;

        var itens = InventoryState.Instance.ItensInventario;

        if (slotSelecionado >= itens.Count)
            return;

        var item = itens[slotSelecionado];

        if (!InventoryState.Instance.ConfirmarOferta(item))
            return;

        telaOferecer.Hide();
        Hide();

        slotSelecionado = -1;
        modoOferecer = false;
    }

    private void CancelarOferta()
    {
        GD.Print("[InventarioUI] CancelarOferta.");

        InventoryState.Instance.CancelarOferta();

        telaOferecer.Hide();
        Hide();

        slotSelecionado = -1;
        modoOferecer = false;
    }

    private void AtualizarInventario()
    {
        GD.Print("[InventarioUI] AtualizarInventario chamado.");

        var itens = InventoryState.Instance.ItensInventario;
        int quantidadeSlots = InventoryState.Instance.QuantidadeSlots;

        GD.Print(
            "[InventarioUI] Quantidade de itens: ",
            itens.Count
        );

        GD.Print(
            "[InventarioUI] Quantidade de slots UI: ",
            slotsInventario.Length
        );

        GD.Print(
            "[InventarioUI] Quantidade de texturas UI: ",
            texturasSlots.Length
        );

        for (int i = 0; i < slotsInventario.Length; i++)
        {
            bool slotDesbloqueado = i < quantidadeSlots;

            slotsInventario[i].Visible = slotDesbloqueado;

            if (!slotDesbloqueado)
            {
                texturasSlots[i].Texture = null;
                texturasSlots[i].Visible = false;
                continue;
            }

            if (i < itens.Count)
            {
                GD.Print(
                    "[InventarioUI] Slot ",
                    i,
                    " -> ",
                    itens[i] != null ? itens[i].Nome : "NULL"
                );

                texturasSlots[i].Texture = itens[i].Textura;
                texturasSlots[i].Visible = true;
            }
            else
            {
                GD.Print(
                    "[InventarioUI] Slot ",
                    i,
                    " -> vazio"
                );

                texturasSlots[i].Texture = null;
                texturasSlots[i].Visible = false;
            }
        }
    }
}