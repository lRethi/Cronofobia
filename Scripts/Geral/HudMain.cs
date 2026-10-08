using Godot;
using System;

public partial class HudMain : Control
{
    private TextureRect[] arrayFome;
    private TextureRect[] arraySede;
    private Label textDinheiro;

    public override void _Ready()
    {
        textDinheiro = GetNode<Label>("objDinheiro");

        arrayFome = new TextureRect[]
        {
            GetNode<TextureRect>("barraFome/fomeTier0"),
            GetNode<TextureRect>("barraFome/fomeTier1"),
            GetNode<TextureRect>("barraFome/fomeTier2"),
            GetNode<TextureRect>("barraFome/fomeTier3")
        };

        arraySede = new TextureRect[]
        {
            GetNode<TextureRect>("barraAgua/aguaTier0"),
            GetNode<TextureRect>("barraAgua/aguaTier1"),
            GetNode<TextureRect>("barraAgua/aguaTier2"),
            GetNode<TextureRect>("barraAgua/aguaTier3")
        };

        NeedsState.Instance.HungerChanged += OnHungerChanged;
        NeedsState.Instance.ThirstChanged += OnThirstChanged;
        NeedsState.Instance.MoneyChanged += OnMoneyChanged;

        AtualizarFome(NeedsState.Instance.varFome);
        AtualizarSede(NeedsState.Instance.varSede);
        AtualizarDinheiro(NeedsState.Instance.varDinheiro);
    }

    private void OnHungerChanged(float newValue)
    {
        AtualizarFome(newValue);
    }

    private void OnThirstChanged(float newValue)
    {
        AtualizarSede(newValue);
    }

    private void OnMoneyChanged(float newValue)
    {
        AtualizarDinheiro(newValue);
    }

    private void AtualizarFome(float value)
    {
        int maximo = NeedsState.Instance.GetMaximoFome();
        int permanentes = arrayFome.Length - maximo;

        for (int i = 0; i < arrayFome.Length; i++)
        {
            if (i < permanentes)
            {
                arrayFome[i].Visible = true;
            }
            else
            {
                int indiceValue = i - permanentes;
                arrayFome[i].Visible = indiceValue < value;
            }
        }
    }

    private void AtualizarSede(float value)
    {
        int maximo = NeedsState.Instance.GetMaximoSede();
        int permanentes = arraySede.Length - maximo;

        for (int i = 0; i < arraySede.Length; i++)
        {
            if (i < permanentes)
            {
                arraySede[i].Visible = true;
            }
            else
            {
                int indiceValue = i - permanentes;
                arraySede[i].Visible = indiceValue < value;
            }
        }
    }

    private void AtualizarDinheiro(float value)
    {
        textDinheiro.Text = $"R${value:0.00}";
    }

    public override void _ExitTree()
    {
        if (NeedsState.Instance != null)
        {
            NeedsState.Instance.HungerChanged -= OnHungerChanged;
            NeedsState.Instance.ThirstChanged -= OnThirstChanged;
            NeedsState.Instance.MoneyChanged -= OnMoneyChanged;
        }
    }
}