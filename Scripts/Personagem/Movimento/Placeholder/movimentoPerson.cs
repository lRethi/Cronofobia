using Godot;
using System;

public partial class movimentoPerson : CharacterBody3D
{
	public const float Speed = 3f;
	public const float JumpVelocity = 5f;

	[Export] public Node cameraNode;
	[Export] public Node3D cameraPivotNode;

	private cameraBonitaDoFred cameraScript;

	public override void _Ready()
	{
		cameraScript = cameraPivotNode as cameraBonitaDoFred;
	}

	public override void _PhysicsProcess(double delta)
	{
		Vector3 velocity = Velocity;
		if (GameState.EmDialogo)
		{
			// continua aplicando gravidade
			if (!IsOnFloor())
				velocity += GetGravity() * (float)delta;
			// trava mov horizontal
			velocity.X = 0;
			velocity.Z = 0;

			Velocity = velocity;
			MoveAndSlide();
			return;
		}

		if (!IsOnFloor())
			velocity += GetGravity() * (float)delta;

		Vector2 inputDir = Input.GetVector("move_left", "move_right", "move_forward", "move_back");

		if (cameraNode == null)
		{
			Velocity = velocity;
			MoveAndSlide();
			return;
		}

		// pega a base da câmera
		float yaw = Mathf.DegToRad(cameraScript.GetCameraYaw());

		Vector3 forward = new Vector3(
			Mathf.Sin(yaw),
			0,
			Mathf.Cos(yaw)
		);

		Vector3 right = new Vector3(
			Mathf.Cos(yaw),
			0,
			-Mathf.Sin(yaw)
		);

		Vector3 direction = (forward * inputDir.Y + right * inputDir.X).Normalized();

		if (direction != Vector3.Zero)
		{
			velocity.X = direction.X * Speed;
			velocity.Z = direction.Z * Speed;
		}
		else
		{
			velocity.X = Mathf.MoveToward(velocity.X, 0, Speed);
			velocity.Z = Mathf.MoveToward(velocity.Z, 0, Speed);
		}

		Velocity = velocity;
		MoveAndSlide();
	}
}
