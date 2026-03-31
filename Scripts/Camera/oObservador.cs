/*
using Godot;

public partial class oObservador : Marker3D
{
	[Export] public Node3D Character;

	[Export] public float TargetSpeed = 5.0f;   // velocidade do "alvo"
	[Export] public float SmoothSpeed = 2.5f;   // velocidade da câmera
	[Export] public float DeadZone = 0.02f;     // zona morta (radianos)

	private float _baseYaw;
	private float _maxOffset;
	private float _currentTargetYaw;

	public override void _Ready()
	{
		_baseYaw = Rotation.Y;
		_currentTargetYaw = _baseYaw;
		_maxOffset = Mathf.DegToRad(45f);
	}

	public override void _Process(double delta)
	{
		Vector3 toPlayer = Character.GlobalPosition - GlobalPosition;
		toPlayer.Y = 0;

		if (toPlayer.LengthSquared() < 0.0001f)
			return;

		toPlayer = toPlayer.Normalized();

		float targetYaw = Mathf.Atan2(toPlayer.X, toPlayer.Z);

		float deltaYaw = Mathf.AngleDifference(_baseYaw, targetYaw);

		deltaYaw = Mathf.Clamp(deltaYaw, -_maxOffset, _maxOffset);

		float clampedYaw = _baseYaw + deltaYaw;

		float diffTarget = Mathf.AngleDifference(_currentTargetYaw, clampedYaw);

		if (Mathf.Abs(diffTarget) > DeadZone)
		{
			float maxStep = TargetSpeed * (float)delta;
			diffTarget = Mathf.Clamp(diffTarget, -maxStep, maxStep);

			_currentTargetYaw += diffTarget;
		}

		Vector3 rot = Rotation;
		rot.Y = Mathf.LerpAngle(rot.Y, _currentTargetYaw, SmoothSpeed * (float)delta);
		Rotation = rot;
	}
}
*/