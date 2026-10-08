using Godot;
using System;
using DialogueManagerRuntime;
using Godot.Collections;
using System.Threading.Tasks;

public partial class GameState : Node
{
    public static GameState Instance { get; private set; }
    public event Action<int, int> WeirdRouteValueChanged;

    public static bool TempoCongelado = false;

    public Dictionary Flags = new();

    private bool dialogoAberto = false;

    public bool DialogoAberto => dialogoAberto;

    [Signal]
    public delegate void FlagChangedEventHandler(string key, bool value);
    public static bool cameraInputEnabled { get; private set; } = true;

    public int weirdRouteValue = 0;

    public override void _EnterTree()
    {
        if (Instance != null && Instance != this)
        {
            QueueFree();
            return;
        }

        Instance = this;
    }

    public override void _Ready()
    {
        DialogueManager.DialogueStarted += OnDialogueStarted;
        DialogueManager.DialogueEnded += OnDialogueEnded;
    }

    public override void _ExitTree()
    {
        DialogueManager.DialogueStarted -= OnDialogueStarted;
        DialogueManager.DialogueEnded -= OnDialogueEnded;
    }

    private void OnDialogueStarted(Resource _)
    {
        dialogoAberto = true;
        TempoCongelado = true;
        Input.MouseMode = Input.MouseModeEnum.Visible;
    }

    private void OnDialogueEnded(Resource _)
    {
        dialogoAberto = false;
        TempoCongelado = false;
        Input.MouseMode = Input.MouseModeEnum.Captured;
    }
    public void SetCameraInputEnabled(bool enabled)
    {
        cameraInputEnabled = enabled;
    }
    
    public void SetCameraMouseCaptured(bool captured)
    {
        Input.MouseMode = captured
            ? Input.MouseModeEnum.Captured
            : Input.MouseModeEnum.Visible;
    }

    public bool GetFlag(string key)
    {
        return Flags.ContainsKey(key) && (bool)Flags[key];
    }

    public void SetFlag(string key, bool value = true)
    {
        bool oldValue = GetFlag(key);

        if (oldValue == value)
            return;

        Flags[key] = value;
        EmitSignal(SignalName.FlagChanged, key, value);
    }

    public void StartQuest(string questId)
    {
        QuestState.Instance.AddQuest(questId);
    }

    public void endingSequence()
    {
        if(weirdRouteValue >= 7) endGame("colapsoDoSistema");
        else if(GetFlag("semtetos_abrigo_inaugurado") && GetFlag("igreja_deus_maquina_concluido") && GetFlag("empregoEstavel") && GetFlag("ultralink_buscaFinalizada") && GetFlag("exposicao")) endGame("desempenhoSatisfatorio");
        else if(GetFlag("exposicao")) endGame("exposicao");
        else if(GetFlag("ultralink_atualizada")) endGame("atualizacaoObrigatoria");
        else if(GetFlag("ultralink_desligada")) endGame("desligamento");
        else if(GetFlag("igreja_deus_maquina_incentivou_culto")) endGame("deusMaquina");
        else if(GetFlag("igreja_deus_maquina_rejeitou_culto")) endGame("assimilacao");
        else if(GetFlag("empregoEstavel")) endGame("empregoEstavel");
        else if(GetFlag("semtetos_abrigo_inaugurado")) endGame("abrigoInconstante");
        else endGame("natal");
    }

    public void endGame(string endingName)
    {
        GetTree().ChangeSceneToFile($"res://Assets/Scenes/Endings/{endingName}.tscn");
    }

    public void AlterarWeirdRouteValue(int novoValor)
    {
        int anterior = weirdRouteValue;

        weirdRouteValue = novoValor;

        if (anterior != novoValor)
        {
            WeirdRouteValueChanged?.Invoke(
                anterior,
                novoValor
            );
        }
    }

    public void AdicionarWeirdRouteValue(int quantidade)
    {
        AlterarWeirdRouteValue(
            weirdRouteValue + quantidade
        );
    }

    public void voltarParaOMenu()
    {
        GetTree().ChangeSceneToFile("res://Assets/Scenes/cenaTitulo.tscn");
        ResetarJogo();
    }

    public void ResetarJogo()
    {
        Flags.Clear();

        weirdRouteValue = 0;

        dialogoAberto = false;

        TempoCongelado = false;
        cameraInputEnabled = true;

        Input.MouseMode = Input.MouseModeEnum.Visible;

        NeedsState.Instance?.Resetar();
        InventoryState.Instance?.Resetar();
        EffectManager.Instance?.Resetar();
        QuestState.Instance?.Resetar();
        TimeState.Instance?.Resetar();
        WorldManager.Instance?.Resetar();
    }
}