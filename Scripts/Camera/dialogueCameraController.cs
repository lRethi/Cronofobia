using Godot;
using DialogueManagerRuntime;

public partial class dialogueCameraController : Node
{
    [Export] public cameraBonitaDoFred GameplayCamera;
    private DialogueMarker3D currentNPC;

    public void StartDialogue(DialogueMarker3D npc)
    {
        if (!IsInstanceValid(npc))
        {
            return;
        }

        if (!IsInstanceValid(GameplayCamera))
        {
            return;
        }

        if (currentNPC == npc)
        {
            return;
        }

        currentNPC = npc;

        GameplayCamera.StartDialogueCamera(
            npc.GlobalPosition,
            0.5f
        );
    }

    public void EndDialogue()
    {
        if (!IsInstanceValid(GameplayCamera))
        {
            return;
        }

        GameplayCamera.EndDialogueCamera(0.5f);

        currentNPC = null;
    }
}