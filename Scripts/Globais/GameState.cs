using Godot;
using DialogueManagerRuntime;
using Godot.Collections;
using System.Threading.Tasks;

public partial class GameState : Node
{
    public static GameState Instance { get; private set; }

    public static bool TempoCongelado = false;

    public Dictionary Flags = new();

    private bool dialogoAberto = false;

    public bool DialogoAberto => dialogoAberto;

    [Signal]
    public delegate void FlagChangedEventHandler(string key, bool value);
    public static bool cameraInputEnabled { get; private set; } = true;

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

    public async Task SetFlag(string key, bool value = true)
    {
        Flags[key] = value;
        EmitSignal(SignalName.FlagChanged, key, value);
        await Task.CompletedTask;
    }

    public void StartQuest(string questId)
    {
        QuestState.Instance.AddQuest(questId);
    }

    public void endGame(string endingName)
    {
        GetTree().ChangeSceneToFile($"res://Assets/Endings/{endingName}.tscn");
    }
}