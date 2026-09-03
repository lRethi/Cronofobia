using Godot;

public partial class inventarioFuncionamento : Control
{
    [Export] public Godot.Collections.Array<Button> slotsInventario;
    [Export] public Godot.Collections.Array<TextureRect> texturasSlots;
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
        for (int i = 0; i < slotsInventario.Count; i++)
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

        AtualizarInventario();
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        if (!@event.IsActionPressed("open_inventory"))
            return;

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

        if (Visible)
        {
            TimeState.Instance.CongelarTempo();
            Input.MouseMode = Input.MouseModeEnum.Visible;
        }
        else
        {
            telaUso.Visible = false;
            telaOferecer.Visible = false;
            slotSelecionado = -1;
            modoOferecer = false;

            TimeState.Instance.DescongelarTempo();
            Input.MouseMode = Input.MouseModeEnum.Captured;
        }
    }

    public void AbrirParaOferecer()
    {
        modoOferecer = true;
        slotSelecionado = -1;

        telaUso.Hide();
        telaOferecer.Hide();

        Show();

        Input.MouseMode = Input.MouseModeEnum.Visible;
    }

    private void SelecionarSlot(int index)
    {
        if (index < 0 || index >= InventoryState.Instance.QuantidadeItens)
            return;

        var itens = InventoryState.Instance.ItensInventario;
        var item = itens[index];

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
        if (slotSelecionado < 0)
            return;

        if (InventoryState.Instance.UsarItem(slotSelecionado))
            FecharTelaUso();
    }

    private void DescartarItem()
    {
        if (slotSelecionado < 0)
            return;

        if (InventoryState.Instance.RemoverItem(slotSelecionado))
            FecharTelaUso();
    }

    private void FecharTelaUso()
    {
        telaUso.Visible = false;
        slotSelecionado = -1;
    }

    private void ConfirmarOferta()
    {
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
        InventoryState.Instance.CancelarOferta();

        telaOferecer.Hide();
        Hide();

        slotSelecionado = -1;
        modoOferecer = false;
    }

    private void AtualizarInventario()
    {
        var itens = InventoryState.Instance.ItensInventario;

        for (int i = 0; i < slotsInventario.Count; i++)
        {
            if (i < itens.Count)
            {
                texturasSlots[i].Texture = itens[i].Textura;
                texturasSlots[i].Visible = true;
            }
            else
            {
                texturasSlots[i].Texture = null;
                texturasSlots[i].Visible = false;
            }
        }
    }
}