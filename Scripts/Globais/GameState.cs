using Godot;
using DialogueManagerRuntime;
using Godot.Collections;
using System.Threading.Tasks;

public partial class GameState : Node
{
    public static GameState Instance { get; private set; }
    public static bool TempoCongelado = false;
    public Dictionary Flags = new();

    [Signal]
    public delegate void FlagChangedEventHandler(string key, bool value);

    public override void _EnterTree()
        {
            if (Instance != null && Instance != this) {
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
        TempoCongelado = true;
        Input.MouseMode = Input.MouseModeEnum.Visible;
    }

    private void OnDialogueEnded(Resource _)
    {
        TempoCongelado = false;
        Input.MouseMode = Input.MouseModeEnum.Captured;
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