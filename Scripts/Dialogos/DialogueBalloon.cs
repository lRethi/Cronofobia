using Godot;
using DialogueManagerRuntime;
using Godot.Collections;
using System.Text.RegularExpressions;

namespace DialogueManagerRuntime;

public partial class DialogueBalloon : CanvasLayer
{
    [Export] public Resource DialogueResource;
    [Export] public string StartFromTitle = "start";
    [Export] public string NextAction = "ui_accept";
    [Export] public string SkipAction = "ui_cancel";
    [Export] public Vector2 ULTRALINKOffset = new Vector2(160, 12);
    private dialogueCameraController DialogueCameraController;

    private Control npcDialogueLayer;
    private PanelContainer npcDialogueBox;
    private RichTextLabel characterName;
    private DialogueLabel dialogueLabel;

    private Control ultralinkDialogueLayer;
    private PanelContainer ultralinkDialogueBox;
    private RichTextLabel ultralinkName;
    private DialogueLabel ultralinkLabel;

    private AnimationPlayer animationPlayer;

    private Control playerResponsesLayer;
    private DialogueResponsesMenu responsesMenu;

    private DialogueLine dialogueLine;
    private Array<Variant> temporaryGameStates = new();

    private DialogueMarker3D currentDialogueMarker;

    private bool isWaitingForInput;

    private bool applyingDialogueLine;

    [Export] public string CancelAction = "cancel_dialogue";

    private int dialogueSession;
    private bool endingDialogue;

    public override void _Ready()
    {
        npcDialogueLayer = GetNode<Control>("%NPCDialogueLayer");
        npcDialogueBox = GetNode<PanelContainer>("%NPCDialogueBox");
        characterName = GetNode<RichTextLabel>("%CharacterName");
        dialogueLabel = GetNode<DialogueLabel>("%DialogueLabel");
        DialogueCameraController = GetNode<dialogueCameraController>("../%DialogueCameraController");

        animationPlayer = GetNode<AnimationPlayer>("%AnimationPlayer");

        ultralinkDialogueLayer = GetNode<Control>("%ULTRALINKDialogueLayer");
        ultralinkDialogueBox = GetNode<PanelContainer>("%ULTRALINKDialogueBox");
        ultralinkName = GetNode<RichTextLabel>("%ULTRALINKName");
        ultralinkLabel = GetNode<DialogueLabel>("%ULTRALINKLabel");

        playerResponsesLayer = GetNode<Control>("%PlayerResponsesLayer");
        responsesMenu = GetNode<DialogueResponsesMenu>("%ResponsesMenu");

        npcDialogueLayer.Hide();
        ultralinkDialogueLayer.Hide();
        playerResponsesLayer.Hide();

        responsesMenu.ResponseSelected += response =>
        {
            EffectManager.Instance?.AoEscolherOpcaoDialogo();

            responsesMenu.Hide();
            playerResponsesLayer.Hide();
            npcDialogueLayer.Hide();
            ultralinkDialogueLayer.Hide();

            Next(response.NextId);
        };

        if (string.IsNullOrEmpty(responsesMenu.NextAction))
        {
            responsesMenu.NextAction = NextAction;
        }
    }

    public override void _Process(double delta)
    {
        if (!IsInstanceValid(currentDialogueMarker))
        {
            return;
        }

        if (!npcDialogueBox.Visible)
        {
            return;
        }

        Vector2 markerPosition =
            currentDialogueMarker.GetPositionInViewport();

        npcDialogueBox.Position =
            markerPosition -
            new Vector2(
                npcDialogueBox.Size.X / 2.0f,
                npcDialogueBox.Size.Y
            );

        if (ultralinkDialogueBox.Visible)
        {
            ultralinkDialogueBox.Position =
                npcDialogueBox.Position +
                new Vector2(
                    -ultralinkDialogueBox.Size.X,
                    npcDialogueBox.Size.Y
                ) +
                ULTRALINKOffset;
        }
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        if (!Visible)
            return;

        if (@event.IsActionPressed(CancelAction))
        {
            GetViewport().SetInputAsHandled();
            CancelDialogue();
            return;
        }

        if (dialogueLabel.IsTyping)
        {
            if (@event.IsActionPressed(SkipAction))
            {
                dialogueLabel.SkipTyping();
                GetViewport().SetInputAsHandled();
            }

            return;
        }

        if (!isWaitingForInput)
            return;

        if (dialogueLine.Responses.Count > 0)
            return;

        if (@event.IsActionPressed(NextAction))
        {
            GetViewport().SetInputAsHandled();
            Next(dialogueLine.NextId);
        }
    }

    private void CancelDialogue()
    {
        if (endingDialogue)
            return;

        if (InventoryState.Instance?.OfertaPendente == true)
            InventoryState.Instance.CancelarOferta();

        DialogueManager.Instance.EmitSignal(
            "dialogue_ended",
            DialogueResource
        );

        EndDialogue();
    }

    public async void Start(
        Resource dialogueResource = null,
        string title = "",
        Array<Variant> extraGameStates = null
    )
    {
        int session = ++dialogueSession;
        endingDialogue = false;

        if (!IsNodeReady())
            await ToSignal(this, SignalName.Ready);

        if (session != dialogueSession)
            return;

        if (IsInstanceValid(dialogueResource))
            DialogueResource = dialogueResource;

        if (!string.IsNullOrEmpty(title))
            StartFromTitle = title;

        if (!IsInstanceValid(DialogueResource))
        {
            GD.PushError(
                "DialogueBalloon: DialogueResource não foi definida."
            );

            return;
        }

        temporaryGameStates = new Array<Variant> { this };

        if (extraGameStates != null)
        {
            foreach (Variant gameState in extraGameStates)
                temporaryGameStates.Add(gameState);
        }

        isWaitingForInput = false;

        dialogueLine = await DialogueManager.GetNextDialogueLine(
            DialogueResource,
            StartFromTitle,
            temporaryGameStates
        );

        if (session != dialogueSession || endingDialogue)
            return;

        Show();
        ApplyDialogueLine();
    }

    public async void Next(string nextId)
    {
        if (endingDialogue)
            return;

        int session = dialogueSession;

        isWaitingForInput = false;

        dialogueLine = await DialogueManager.GetNextDialogueLine(
            DialogueResource,
            nextId,
            temporaryGameStates
        );

        if (session != dialogueSession || endingDialogue)
            return;

        ApplyDialogueLine();
    }

    private async void ApplyDialogueLine()
    {
        if (applyingDialogueLine)
        {
            return;
        }

        int session = dialogueSession;

        applyingDialogueLine = true;

        try
        {
            if (dialogueLine == null)
            {
                EndDialogue();
                return;
            }

            currentDialogueMarker = null;

            if (!string.IsNullOrEmpty(dialogueLine.Character))
            {
                currentDialogueMarker =
                    DialogueMarker3D.FindForCharacter(
                        dialogueLine.Character
                    );
            }

            if (IsInstanceValid(currentDialogueMarker))
            {
                DialogueCameraController.StartDialogue(
                    currentDialogueMarker
                );
            }

            isWaitingForInput = false;

            npcDialogueLayer.Hide();
            ultralinkDialogueLayer.Hide();
            playerResponsesLayer.Hide();

            dialogueLabel.Hide();
            ultralinkLabel.Hide();
            responsesMenu.Hide();

            if (!string.IsNullOrEmpty(dialogueLine.Character))
            {
                characterName.Text =
                    Tr(dialogueLine.Character, "dialogue");

                dialogueLabel.DialogueLine = dialogueLine;

                npcDialogueLayer.Show();
                dialogueLabel.Show();

                animationPlayer.Play("NPC_In");
            }

            foreach (DialogueLine concurrentLine in dialogueLine.ConcurrentLines)
            {
                if (concurrentLine.Character != "ULTRALINK")
                {
                    continue;
                }

                ExtractULTRALINKFace(concurrentLine);

                ultralinkLabel.DialogueLine = concurrentLine;

                ultralinkDialogueLayer.Show();
                ultralinkLabel.Show();

                animationPlayer.Play("ULTRALINK_In");

                break;
            }

            responsesMenu.Responses = dialogueLine.Responses;

            if (dialogueLine.Responses.Count > 0)
            {
                playerResponsesLayer.Show();
            }

            if (!string.IsNullOrEmpty(dialogueLine.Text))
            {
                dialogueLabel.TypeOut();

                await ToSignal(
                    dialogueLabel,
                    DialogueLabel.SignalName.FinishedTyping
                );
            }

            if (session != dialogueSession || endingDialogue)
                    return;

            if (
                ultralinkDialogueLayer.Visible &&
                !string.IsNullOrEmpty(
                    ultralinkLabel.DialogueLine.Text
                )
            )
            {
                ultralinkLabel.TypeOut();

                await ToSignal(
                    ultralinkLabel,
                    DialogueLabel.SignalName.FinishedTyping
                );
            }

            if (session != dialogueSession || endingDialogue)
                return;

            if (dialogueLine.Responses.Count > 0)
            {
                responsesMenu.Show();
                isWaitingForInput = false;
                return;
            }

            isWaitingForInput = true;
        }
        finally
        {
            applyingDialogueLine = false;
        }
    }

    private async void EndDialogue()
    {
        if (endingDialogue)
            return;

        endingDialogue = true;
        dialogueSession++;

        isWaitingForInput = false;

        if (dialogueLabel != null && dialogueLabel.IsTyping)
            dialogueLabel.SkipTyping();

        if (ultralinkLabel != null && ultralinkLabel.IsTyping)
            ultralinkLabel.SkipTyping();

        DialogueCameraController.EndDialogue();

        animationPlayer.Play("NPC_Out");

        if (ultralinkDialogueLayer.Visible)
            animationPlayer.Play("ULTRALINK_Out");

        await ToSignal(
            animationPlayer,
            AnimationPlayer.SignalName.AnimationFinished
        );

        if (!IsInstanceValid(this))
            return;

        npcDialogueLayer.Hide();
        ultralinkDialogueLayer.Hide();
        playerResponsesLayer.Hide();

        QueueFree();
    }

    private void ExtractULTRALINKFace(DialogueLine line)
    {
        Match match = Regex.Match(
            line.Text,
            @"^\{(.*?)\}\s*"
        );

        if (!match.Success)
        {
            ultralinkName.Text = "ULTRALINK";
            return;
        }

        ultralinkName.Text = match.Groups[1].Value;

        line.Text = line.Text.Substring(match.Length);
    }
}