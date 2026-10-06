using Godot;
using DialogueManagerRuntime;

public partial class dialogueCameraController : Node
{
    private DialogueMarker3D currentNPC;
    private cameraBonitaDoFred gameplayCamera;

    public void StartDialogue(DialogueMarker3D npc)
    {
        if (!IsInstanceValid(npc))
            return;

        if (WorldManager.Instance == null)
            return;

        WorldArea areaAtual = WorldManager.Instance.AreaAtual;

        if (!IsInstanceValid(areaAtual))
            return;

        if (!IsInstanceValid(areaAtual.camera))
            return;

        if (currentNPC == npc)
            return;

        gameplayCamera = areaAtual.camera;
        currentNPC = npc;

        gameplayCamera.StartDialogueCamera(
            npc,
            0.5f
        );
    }

    public void EndDialogue()
    {
        if (!IsInstanceValid(gameplayCamera))
        {
            currentNPC = null;
            return;
        }

        gameplayCamera.EndDialogueCamera(0.5f);

        gameplayCamera = null;
        currentNPC = null;
    }
}