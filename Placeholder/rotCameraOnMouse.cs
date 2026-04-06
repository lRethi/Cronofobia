using Godot;
using PhantomCamera;
using PhantomCamera.Manager;
using System;

public partial class rotCameraOnMouse : Node3D
{
	[Export] Node cameraNode;
	float sensibilidade = 0.1f;
	float yaw;
	float pitch;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Input.MouseMode = Input.MouseModeEnum.Captured;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Input(InputEvent @event)
	{
		if (GameState.EmDialogo)
			return;

		if (@event is InputEventMouseMotion motion)
		{
			yaw += -motion.Relative.X * sensibilidade;
			pitch -= motion.Relative.Y * sensibilidade;
			yaw = Mathf.Wrap(yaw, -180f, 180f);
			pitch = Mathf.Clamp(pitch, -35f, 15f);
		}
	}

	public override void _Process(double delta)
	{
		if (cameraNode == null)
		{
			GD.Print("Camera NULL");
			return;
		}
		cameraNode.Call("set_third_person_rotation_degrees", new Vector3(pitch, yaw, 0));
	}
}
