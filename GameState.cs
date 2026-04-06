using Godot;
using DialogueManagerRuntime;
using Godot.Collections;
using System.Threading.Tasks;

public partial class GameState : Node
{
    public static bool EmDialogo = false;
    public Dictionary Flags = new();

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
        EmDialogo = true;
        Input.MouseMode = Input.MouseModeEnum.Visible;
    }

    private void OnDialogueEnded(Resource _)
    {
        EmDialogo = false;
        Input.MouseMode = Input.MouseModeEnum.Captured;
    }

    public bool GetFlag(string key)
    {
        return Flags.ContainsKey(key) && (bool)Flags[key];
    }

    public async Task SetFlag(string key, bool value = true)
    {
        Flags[key] = value;
        await Task.CompletedTask;
    }
}