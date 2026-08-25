using Godot;

public partial class inimPerseguirProta : CharacterBody3D
{
	private enum EnemyState
	{
		Idle,
		Chase,
		Search
	}

	private EnemyState estadoAtual = EnemyState.Idle;

	[Export] public Node3D player;
	[Export] public float speed = 1.5f;
	[Export] public float visionRange = 5f;
	[Export] public float visionAngle = 45f;
	[Export] public float lostSightDelay = 0.25f;

	private NavigationAgent3D agent;
	private Area3D alertArea;

	private Vector3 ultimaPosicaoConhecida;
	private float tempoSemVisao = 0f;

	private bool playerNaArea = false;

	private Area3D collisionArea;

	[Export] public float tempoDeVida = 10f;

	[Signal]
	public delegate void PlayerColidiuEventHandler();

	public override void _Ready()
	{
		agent = GetNode<NavigationAgent3D>("NavigationAgent3D");
		alertArea = GetNode<Area3D>("AlertArea");
		collisionArea = GetNode<Area3D>("CollisionArea");

		agent.TargetDesiredDistance = 0.05f;
		agent.PathDesiredDistance = 0.05f;

		alertArea.BodyEntered += OnBodyEntered;
		alertArea.BodyExited += OnBodyExited;

		collisionArea.BodyEntered += OnCollisionAreaBodyEntered;

		GetTree().CreateTimer(tempoDeVida).Timeout += Desaparecer;
	}

	void Desaparecer()
	{
		QueueFree();
	}

	public override void _PhysicsProcess(double delta)
	{
		float dt = (float)delta;

		switch (estadoAtual)
		{
			case EnemyState.Idle:
				EstadoIdle(dt);
				break;

			case EnemyState.Chase:
				EstadoChase(dt);
				break;

			case EnemyState.Search:
				EstadoSearch(dt);
				break;
		}
	}

	void EstadoIdle(float dt)
	{
		Velocity = Vector3.Zero;
		MoveAndSlide();

		if (PodeVerPlayer())
		{
			estadoAtual = EnemyState.Chase;
			tempoSemVisao = 0f;
			ultimaPosicaoConhecida = player.GlobalTransform.Origin;
		}
	}

	void EstadoChase(float dt)
	{
		if (PodeVerPlayer())
		{
			tempoSemVisao = 0f;
			ultimaPosicaoConhecida = player.GlobalTransform.Origin;
			agent.TargetPosition = ultimaPosicaoConhecida;
			MoveToTarget();
			return;
		}

		tempoSemVisao += dt;

		if (playerNaArea)
		{
			agent.TargetPosition = ultimaPosicaoConhecida;
			MoveToTarget();
			return;
		}

		if (tempoSemVisao >= lostSightDelay)
			estadoAtual = EnemyState.Search;
	}

	void EstadoSearch(float dt)
	{
		agent.TargetPosition = ultimaPosicaoConhecida;
		MoveToTarget();

		if (PodeVerPlayer())
		{
			estadoAtual = EnemyState.Chase;
			tempoSemVisao = 0f;
			return;
		}

		if (agent.IsNavigationFinished())
			estadoAtual = EnemyState.Idle;
	}

	void MoveToTarget()
	{
		Vector3 target = ultimaPosicaoConhecida;
		Vector3 current = GlobalTransform.Origin;

		Vector3 next = agent.GetNextPathPosition();
		Vector3 dir = next - current;

		dir.Y = 0f;

		if (dir.LengthSquared() < 0.0001f)
		{
			dir = target - current;
			dir.Y = 0f;

			if (dir.LengthSquared() < 0.0001f)
			{
				Velocity = Vector3.Zero;
				MoveAndSlide();
				return;
			}
		}

		dir = dir.Normalized();

		Velocity = new Vector3(dir.X * speed, Velocity.Y, dir.Z * speed);
		MoveAndSlide();

		Vector3 lookTarget = current + dir;
		if (!lookTarget.IsEqualApprox(current))
			LookAt(lookTarget, Vector3.Up);
	}

	bool PodeVerPlayer()
	{
		if (player == null)
			return false;

		Vector3 origem = GlobalTransform.Origin;
		Vector3 alvo = player.GlobalTransform.Origin;

		Vector3 dir = alvo - origem;
		float dist = dir.Length();

		if (dist > visionRange)
			return false;

		dir = dir.Normalized();

		Vector3 forward = -GlobalTransform.Basis.Z;
		float dot = Mathf.Clamp(forward.Dot(dir), -1f, 1f);
		float angulo = Mathf.RadToDeg(Mathf.Acos(dot));

		if (angulo > visionAngle)
			return false;

		var space = GetWorld3D().DirectSpaceState;
		var query = PhysicsRayQueryParameters3D.Create(origem, alvo);

		query.Exclude = new Godot.Collections.Array<Rid> { GetRid() };

		var result = space.IntersectRay(query);

		if (result.Count == 0)
			return false;

		var collider = result["collider"].As<Node>();

		return collider == player || (collider != null && player.IsAncestorOf(collider));
	}

	void OnBodyEntered(Node body)
	{
		if (body == player)
			playerNaArea = true;
	}

	void OnBodyExited(Node body)
	{
		if (body == player)
			playerNaArea = false;
	}

	void OnCollisionAreaBodyEntered(Node body)
	{
		if (body != player)
			return;

		if (estadoAtual == EnemyState.Chase || estadoAtual == EnemyState.Search)
			EmitSignal(SignalName.PlayerColidiu);
	}
}