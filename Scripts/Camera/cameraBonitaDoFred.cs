using Godot;
using PhantomCamera;

public partial class cameraBonitaDoFred : Node3D
{
    private PhantomCamera3D _pcam;
    private Tween cameraTween;

    [Signal]
    public delegate void CameraChangedEventHandler(int yaw, int pitch);

    [Export]
    private Node3D player;

    [Export]
    private float mouseSensitivity = 0.15f;

    private readonly float basePitch = 0f;

    private float yawRotation = 0f;
    private int lastYawState;

    private Vector3 currentRotation;
    private Vector3 currentOffset;

    private bool dialogueMode = false;

    private Vector3 dialogueRotation;
    private Vector3 dialogueOffset;

    private Vector3 savedRotation;
    private Vector3 savedOffset;

    public float GetCameraYaw()
    {
        return yawRotation;
    }

    public int GetYawState()
    {
        return Mathf.PosMod(
            Mathf.RoundToInt(yawRotation / 90f),
            4
        );
    }

    public override async void _Ready()
    {
        await ToSignal(
            GetTree(),
            SceneTree.SignalName.ProcessFrame
        );

        _pcam = GetNode<Node3D>("%PhantomCamera3D").AsPhantomCamera3D();

        if (player == null)
            player = GetNode<Node3D>("%charGeraldoSalvador");

        currentRotation = _pcam.GetThirdPersonRotationDegrees();
        currentOffset = _pcam.FollowOffset;

        yawRotation = currentRotation.Y;
        lastYawState = GetYawState();

        GameState.Instance.SetCameraInputEnabled(true);
        GameState.Instance.SetCameraMouseCaptured(true);
    }

    public override void _Input(InputEvent @event)
    {
        if (!GameState.cameraInputEnabled || dialogueMode)
            return;

        if (@event is InputEventMouseMotion mouseMotion)
        {
            yawRotation -= mouseMotion.Relative.X * mouseSensitivity;
            UpdateCameraRotation();
        }
    }

    public void StartDialogueCamera(Vector3 npcPosition, float duration)
    {
        if (!dialogueMode)
        {
            savedRotation = currentRotation;
            savedOffset = currentOffset;

            dialogueMode = true;

            GameState.Instance.SetCameraInputEnabled(false);
        }

        cameraTween?.Kill();

        Vector3 relativePosition =
            player.GlobalTransform.Basis.Inverse() *
            (npcPosition - player.GlobalPosition);

        float side = relativePosition.X >= 0f ? 1f : -1f;

        float depth =
            relativePosition.Z >= 0f ? 0.9f : -0.9f;

        dialogueOffset = new Vector3(
            0.65f * side,
            0.1f,
            depth
        );

        dialogueRotation = new Vector3(
            2.5f,
            currentRotation.Y - (5f * side),
            0f
        );

        Vector3 startRotation = currentRotation;
        Vector3 startOffset = currentOffset;

        cameraTween = CreateTween();
        cameraTween.SetParallel(true);

        cameraTween.TweenMethod(
            Callable.From<Vector3>(rotation =>
            {
                currentRotation = rotation;
                _pcam.SetThirdPersonRotationDegrees(rotation);
            }),
            startRotation,
            dialogueRotation,
            duration
        );

        cameraTween.TweenMethod(
            Callable.From<Vector3>(offset =>
            {
                currentOffset = offset;
                _pcam.FollowOffset = offset;
            }),
            startOffset,
            dialogueOffset,
            duration
        );

        cameraTween.SetEase(Tween.EaseType.InOut);
        cameraTween.SetTrans(Tween.TransitionType.Cubic);

        cameraTween.Finished += () =>
        {
            currentRotation = dialogueRotation;
            currentOffset = dialogueOffset;
        };
    }

    public void EndDialogueCamera(float duration)
    {
        cameraTween?.Kill();

        Vector3 startRotation = currentRotation;
        Vector3 startOffset = currentOffset;

        cameraTween = CreateTween();
        cameraTween.SetParallel(true);

        cameraTween.TweenMethod(
            Callable.From<Vector3>(rotation =>
            {
                currentRotation = rotation;
                _pcam.SetThirdPersonRotationDegrees(rotation);
            }),
            startRotation,
            savedRotation,
            duration
        );

        cameraTween.TweenMethod(
            Callable.From<Vector3>(offset =>
            {
                currentOffset = offset;
                _pcam.FollowOffset = offset;
            }),
            startOffset,
            savedOffset,
            duration
        );

        cameraTween.SetEase(Tween.EaseType.InOut);
        cameraTween.SetTrans(Tween.TransitionType.Cubic);

        cameraTween.Finished += () =>
        {
            currentRotation = savedRotation;
            currentOffset = savedOffset;
            yawRotation = savedRotation.Y;
            dialogueMode = false;

            GameState.Instance.SetCameraInputEnabled(true);
            GameState.Instance.SetCameraMouseCaptured(true);
        };
    }

    private void UpdateCameraRotation()
    {
        currentRotation = new Vector3(
            basePitch,
            yawRotation,
            0f
        );

        _pcam.SetThirdPersonRotationDegrees(currentRotation);

        int newYawState = GetYawState();

        if (newYawState != lastYawState)
        {
            lastYawState = newYawState;

            EmitSignal(
                SignalName.CameraChanged,
                newYawState,
                0
            );
        }
    }
}