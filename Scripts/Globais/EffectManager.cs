using Godot;
using System;
using System.Collections.Generic;

public partial class EffectManager : Node
{
    public static EffectManager Instance { get; private set; }

    public const string IdGalaoAgua = "galao_agua";
    public const string IdGarrafaAgua = "garrafa_agua";
    public const string IdLataFeijao = "lata_feijao";
    public const string IdMarmitta = "marmita";
    public const string IdFeijoada = "feijoada";
	public const string IdBlockInventario = "block_inventario";

    [Export]
    public ItemDefinition[] itensCatalogo = Array.Empty<ItemDefinition>();

    private readonly List<EffectBase> efeitosDisponiveis = new();
    private readonly List<EffectBase> efeitosAtivos = new();

    private bool erroNoSistemaPendente;
    private bool bloqueouNovasManutencoes;

    public IReadOnlyList<EffectBase> EfeitosAtivos => efeitosAtivos;

    public bool BloqueouNovasManutencoes =>
        bloqueouNovasManutencoes;

    public override void _EnterTree()
    {
        if (Instance != null && Instance != this)
        {
            QueueFree();
            return;
        }

        Instance = this;
    }

    public override void _ExitTree()
    {
        if (GameState.Instance != null)
            GameState.Instance.WeirdRouteValueChanged -= OnWeirdRouteValueChanged;

        if (Instance == this)
            Instance = null;
    }

    public override void _Ready()
    {
        efeitosDisponiveis.Add(new MaisFomeMenosSede());
        efeitosDisponiveis.Add(new MaisSedeMenosFome());
        efeitosDisponiveis.Add(new DinheiroEmComida());
        efeitosDisponiveis.Add(new Ima());
        efeitosDisponiveis.Add(new TransformadorBionico());
        efeitosDisponiveis.Add(new DetectorMetais());
        efeitosDisponiveis.Add(new ValeAlimentacaoVirtual());
        efeitosDisponiveis.Add(new TransacaoAutomatica());
        efeitosDisponiveis.Add(new AlteracaoDeAlarme());
        efeitosDisponiveis.Add(new MochilaModerna());
        efeitosDisponiveis.Add(new PeleEndotermica());
        efeitosDisponiveis.Add(new TenisDinamico());
        efeitosDisponiveis.Add(new VelocidadePelaSede());
        efeitosDisponiveis.Add(new NovosImpostos());
        efeitosDisponiveis.Add(new ExtensorFragil());
        efeitosDisponiveis.Add(new NeurochipDeCozinha());
        efeitosDisponiveis.Add(new ErroNoSistema());

        if (GameState.Instance != null)
            GameState.Instance.WeirdRouteValueChanged += OnWeirdRouteValueChanged;
    }

    public bool TemEfeito<T>() where T : EffectBase
    {
        Type tipo = typeof(T);

        foreach (EffectBase efeito in efeitosAtivos)
        {
            if (efeito.GetType() == tipo)
                return true;
        }

        return false;
    }

    public bool TemEfeito(Type tipo)
    {
        if (tipo == null)
            return false;

        foreach (EffectBase efeito in efeitosAtivos)
        {
            if (efeito.GetType() == tipo)
                return true;
        }

        return false;
    }

    public void AdicionarEfeito(EffectBase efeito)
    {
        if (efeito == null)
            return;

        if (TemEfeito(efeito.GetType()))
            return;

        if (efeito is ErroNoSistema)
            erroNoSistemaPendente = false;

        efeito.aoEscolher();

        efeitosAtivos.Add(efeito);
    }

    public List<EffectBase> GerarOpcoes()
    {
        if (bloqueouNovasManutencoes)
            return new List<EffectBase>();

        List<EffectBase> copiaEfeitos = new();

        foreach (EffectBase efeito in efeitosDisponiveis)
        {
            if (!efeito.PodeAparecerAleatoriamente())
                continue;

            if (TemEfeito(efeito.GetType()))
                continue;

            if (!efeito.PodeEscolher())
                continue;

            copiaEfeitos.Add(efeito);
        }

        copiaEfeitos.Shuffle();

        List<EffectBase> opcoes = new();

        if (erroNoSistemaPendente)
        {
            opcoes.Add(new ErroNoSistema());
        }

        int quantidadeAleatoria =
            erroNoSistemaPendente
                ? 2
                : 3;

        for (int i = 0;
             i < quantidadeAleatoria && i < copiaEfeitos.Count;
             i++)
        {
            opcoes.Add(copiaEfeitos[i]);
        }

        return opcoes;
    }

    public void AplicarEfeitosAleatorios(int quantidade)
    {
        if (quantidade <= 0)
            return;

        List<EffectBase> disponiveis = new();

        foreach (EffectBase efeito in efeitosDisponiveis)
        {
            if (!efeito.PodeAparecerAleatoriamente())
                continue;

            if (TemEfeito(efeito.GetType()))
                continue;

            if (!efeito.PodeEscolher())
                continue;

            disponiveis.Add(efeito);
        }

        disponiveis.Shuffle();

        quantidade = Mathf.Min(
            quantidade,
            disponiveis.Count
        );

        for (int i = 0; i < quantidade; i++)
            AdicionarEfeito(disponiveis[i]);
    }

    public void InicioDoDia()
    {
        foreach (EffectBase efeito in efeitosAtivos)
            efeito.InicioDoDia();
    }

    public void FimDoDia()
    {
        foreach (EffectBase efeito in efeitosAtivos)
            efeito.FimDoDia();
    }

    public float AplicarVelocidade(float velocidade)
    {
        float multiplicador = 1f;
        float bonus = 0f;

        foreach (EffectBase efeito in efeitosAtivos)
        {
            multiplicador *= efeito.MultiplicadorVelocidade();
            bonus += efeito.BonusVelocidade();
        }

        return Mathf.Max(
            0f,
            velocidade * multiplicador + bonus
        );
    }

    public float AplicarPreco(string itemId, float preco)
    {
        if (string.IsNullOrEmpty(itemId))
            return preco;

        foreach (EffectBase efeito in efeitosAtivos)
        {
            if (efeito.PodeComprarGratis(itemId))
                return 0f;
        }

        float resultado = preco;

        foreach (EffectBase efeito in efeitosAtivos)
            resultado *= efeito.MultiplicadorPreco(itemId);

        return Mathf.Max(0f, resultado);
    }

    public void CompraConcluida(string itemId)
    {
        foreach (EffectBase efeito in efeitosAtivos)
            efeito.CompraConcluida(itemId);
    }

    public float GetInteractionRange(float alcanceBase)
    {
        float multiplicador = 1f;

        foreach (EffectBase efeito in efeitosAtivos)
            multiplicador *= efeito.MultiplicadorAlcanceInteracao();

        return alcanceBase * multiplicador;
    }

    public int GetBonusSlotsInventario()
    {
        int bonus = 0;

        foreach (EffectBase efeito in efeitosAtivos)
            bonus += efeito.BonusSlotsInventario();

        return bonus;
    }

    public bool PermiteDormirSemLugar()
    {
        foreach (EffectBase efeito in efeitosAtivos)
        {
            if (efeito.PermiteDormirSemLugar())
                return true;
        }

        return false;
    }

    public bool PermiteTransformarAguaComida()
    {
        foreach (EffectBase efeito in efeitosAtivos)
        {
            if (efeito.PermiteTransformarAguaComida())
                return true;
        }

        return false;
    }

    public bool PermiteCozinharFeijoada()
    {
        foreach (EffectBase efeito in efeitosAtivos)
        {
            if (efeito.PermiteCozinharFeijoada())
                return true;
        }

        return false;
    }

    public bool DeveDescartarFeijaoAoPegar(string itemId)
    {
        foreach (EffectBase efeito in efeitosAtivos)
        {
            if (efeito.DescartarFeijaoAoPegar(itemId))
                return true;
        }

        return false;
    }

    public void AoEscolherOpcaoDialogo()
    {
        foreach (EffectBase efeito in efeitosAtivos)
            efeito.AoEscolherOpcaoDialogo();
    }

    public ItemDefinition ObterItemPorId(string itemId)
    {
        if (string.IsNullOrEmpty(itemId))
            return null;

        foreach (ItemDefinition item in itensCatalogo)
        {
            if (item != null && item.Id == itemId)
                return item;
        }

        return null;
    }

    public void BloquearNovasManutencoes()
    {
        bloqueouNovasManutencoes = true;
        erroNoSistemaPendente = false;
    }

    private void OnWeirdRouteValueChanged(
        int valorAnterior,
        int novoValor
    )
    {
        if (bloqueouNovasManutencoes)
            return;

        if (erroNoSistemaPendente)
            return;

        if (TimeState.Instance == null)
            return;

        if (TimeState.Instance.noiteFinalizada)
            return;

        if (novoValor <= valorAnterior)
            return;

        erroNoSistemaPendente = true;
    }
}

public static class ListExtensions
{
    public static void Shuffle<T>(this IList<T> list)
    {
        Random random = Random.Shared;

        for (int i = list.Count - 1; i > 0; i--)
        {
            int j = random.Next(i + 1);
            (list[i], list[j]) = (list[j], list[i]);
        }
    }
}