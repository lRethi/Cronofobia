using Godot;
using PhantomCamera;

public partial class cameraBonitaDoFred : Node3D
{
    private PhantomCamera3D _pcam;

    private Tween cameraTween;
    [Signal]
    public delegate void CameraChangedEventHandler(int yaw, int pitch);

    [Export]
    private float rotationDuration = 0.25f;

    [Export]
    private Node3D player;

    private readonly float basePitch = 0f;

    private int pitchState = 0;
    private int yawRotation = 0;

    private Vector3 currentRotation;
    private Vector3 targetRotation;

    private Vector3 currentOffset;
    private Vector3 targetOffset;

	private Camera3D objCamera;
    private bool dialogueMode = false;
    private Vector3 dialogueRotation;
    private Vector3 dialogueOffset;
    private Vector3 savedRotation;
    private Vector3 savedOffset;
    private float savedFov;

	public float GetCameraYaw()
	{
		return yawRotation * 90f;
	}
    public int GetYawState()
    {
        return Mathf.PosMod(yawRotation, 4);
    }
    public int GetPitchState()
    {
        return pitchState;
    }

    private readonly Vector3[] pitchOffsets =
    {
        new Vector3(0f, -2f, -0.8f),
        new Vector3(0f, 0.15f, 0f),
        new Vector3(0f, 1.8f, -0.35f)
    };

	private float currentFov;
	private float targetFov;

	private readonly float[] pitchFovs =
	{
		90f,
		75f,
		90f
	};

    public override async void _Ready()
    {
        await ToSignal(GetTree(), SceneTree.SignalName.ProcessFrame);

        _pcam = GetNode<Node3D>("%PhantomCamera3D").AsPhantomCamera3D();
		objCamera = GetNode<Camera3D>("../Camera3D");

        currentRotation = _pcam.GetThirdPersonRotationDegrees();
        targetRotation = currentRotation;

        currentOffset = _pcam.FollowOffset;
        targetOffset = currentOffset;

        yawRotation = Mathf.RoundToInt(currentRotation.Y / 90f);
        pitchState = Mathf.Clamp(
            Mathf.RoundToInt((currentRotation.X - basePitch) / 90f),
            -1,
            1
        );

		currentFov = objCamera.Fov;
		targetFov = currentFov;
    }

    public override void _Input(InputEvent @event)
    {
        if (dialogueMode) return;

        if (@event.IsActionPressed("cam_left"))
        {
            yawRotation++;
			objCamera.Fov = 75f;
            UpdateTarget();
        }
        else if (@event.IsActionPressed("cam_right"))
        {
            yawRotation--;
			objCamera.Fov = 75f;
            UpdateTarget();
        }
        else if (@event.IsActionPressed("cam_back"))
        {
            pitchState = Mathf.Clamp(pitchState + 1, -1, 1);
			objCamera.Fov = 90f;
            UpdateTarget();
        }
        else if (@event.IsActionPressed("cam_forward"))
        {
            pitchState = Mathf.Clamp(pitchState - 1, -1, 1);
			objCamera.Fov = 90f;
            UpdateTarget();
        }
    }

    public void StartDialogueCamera(Vector3 npcPosition, float duration)
    {

        if (!dialogueMode)
        {
            savedRotation = currentRotation;
            savedOffset = currentOffset;
            savedFov = currentFov;

            dialogueMode = true;
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
            dialogueMode = false;
        };
    }

    private void UpdateTarget()
    {
        targetRotation = new Vector3(
            basePitch + pitchState * 90f,
            yawRotation * 90f,
            0f
        );

		targetOffset = GetRotatedOffset(pitchOffsets[pitchState + 1]);

		targetFov = pitchFovs[pitchState + 1];

        AnimateCamera();
    }

	private Vector3 GetRotatedOffset(Vector3 offset)
	{
		float yawRadians = Mathf.DegToRad(yawRotation * 90f);

		float x = offset.X * Mathf.Cos(yawRadians) + offset.Z * Mathf.Sin(yawRadians);
		float z = offset.X * Mathf.Sin(yawRadians) + offset.Z * Mathf.Cos(yawRadians);

		return new Vector3(
			x,
			offset.Y,
			z
		);
	}

    private void AnimateCamera()
    {
        cameraTween?.Kill();

        Vector3 startRotation = currentRotation;
        Vector3 endRotation = targetRotation;

        Vector3 startOffset = currentOffset;
        Vector3 endOffset = targetOffset;

        cameraTween = CreateTween();

        cameraTween.SetParallel(true);

        cameraTween.TweenMethod(
            Callable.From<Vector3>(rotation =>
            {
                currentRotation = rotation;
                _pcam.SetThirdPersonRotationDegrees(rotation);
            }),
            startRotation,
            endRotation,
            rotationDuration
        );

        cameraTween.TweenMethod(
            Callable.From<Vector3>(offset =>
            {
                currentOffset = offset;
                _pcam.FollowOffset = offset;
            }),
            startOffset,
            endOffset,
            rotationDuration
        );

		float startFov = currentFov;
		float endFov = targetFov;

		cameraTween.TweenMethod(
			Callable.From<float>(fov =>
			{
				currentFov = fov;
				objCamera.Fov = fov;
			}),
			startFov,
			endFov,
			rotationDuration
		);

        cameraTween.SetEase(Tween.EaseType.InOut);
        cameraTween.SetTrans(Tween.TransitionType.Cubic);

        cameraTween.Finished += () =>
        {
            currentRotation = endRotation;
            currentOffset = endOffset;
            EmitSignal(SignalName.CameraChanged, GetYawState(), pitchState);
        };
    }
}