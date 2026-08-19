using System.Collections.Generic;
using Godot;

public partial class DirectionalSprite : RigidBody3D
{
	[Export]
	private DirectionalSpriteResource directionalTextures;

	private cameraBonitaDoFred cameraScript;

	[Export]
	private Sprite3D sprite;

	[Export]
	public SpriteDirection FacingDirection = SpriteDirection.Front;

	public override void _Ready()
	{
		cameraScript = GetNode<cameraBonitaDoFred>("CameraPivot");
		cameraScript.CameraChanged += atualizarSprite;
		atualizarSprite(cameraScript.GetYawState(), cameraScript.GetPitchState());
	}

	private Texture2D GetTexture(SpriteDirection direction)
	{
		return direction switch
		{
			SpriteDirection.Back => directionalTextures.Back,
			SpriteDirection.Right => directionalTextures.Right,
			SpriteDirection.Front => directionalTextures.Front,
			SpriteDirection.Left => directionalTextures.Left,
			_ => null
		};
	}

	public void atualizarSprite(int cameraYaw, int cameraPitch)
	{
		int horizontal = (cameraYaw - (int)FacingDirection + 4) % 4;
		SpriteDirection direction = (SpriteDirection)horizontal;

		sprite.Texture = GetTexture(direction);
	}
	public override void _ExitTree()
	{
		if (cameraScript != null)
		{
			cameraScript.CameraChanged -= atualizarSprite;
		}
	}
}
public enum SpriteDirection
{
	Back = 0,
	Right = 1,
	Front = 2,
	Left = 3 
}

public enum SpritePitch
{
	Up = 0,
	Middle = 1,
	Down = 2
}
