using Godot;
using System.Collections.Generic;

public partial class Gatekeep : Node
{
    [Export]
    public string removalOnFlag;

    [Export]
    public string showWhenFlag;

    [Export]
    public string hideWhenFlag;

    [Export]
    public bool usarHorario;

    [Export]
    public float horarioInicio = 480f;

    [Export]
    public float horarioFim = 1320f;

    [Export]
    public bool usarDia;

    [Export]
    public int diaMinimo = 1;

    [Export]
    public int diaMaximo = 7;

    [Export]
    public bool usarDayState;

    [Export]
    public DayState dayState;

    private Node target;

    private readonly List<CollisionObject3D> collisionObjects = new();
    private readonly Dictionary<CollisionObject3D, uint> originalCollisionLayers = new();

    public override void _Ready()
    {
        target = EncontrarTarget();

        if (target == null)
            return;

        CacheCollisionObjects(target);

        if (GameState.Instance != null)
            GameState.Instance.FlagChanged += OnFlagChanged;

        if (TimeState.Instance != null)
        {
            TimeState.Instance.TimeChanged += OnTimeChanged;
            TimeState.Instance.DayChanged += OnDayChanged;
            TimeState.Instance.DayStateChanged += OnDayStateChanged;
        }

        UpdateGatekeep();
    }

    public override void _ExitTree()
    {
        if (GameState.Instance != null)
            GameState.Instance.FlagChanged -= OnFlagChanged;

        if (TimeState.Instance != null)
        {
            TimeState.Instance.TimeChanged -= OnTimeChanged;
            TimeState.Instance.DayChanged -= OnDayChanged;
            TimeState.Instance.DayStateChanged -= OnDayStateChanged;
        }
    }

    private Node EncontrarTarget()
    {
        if (ContemPickup(this))
            return this;

        Node pai = GetParent();

        if (pai is Node3D)
            return pai;

        if (pai != null)
        {
            foreach (Node child in pai.GetChildren())
            {
                if (child == this)
                    continue;

                if (child is Node3D node3D && ContemCharacterBody3D(node3D))
                    return node3D;
            }

            foreach (Node child in pai.GetChildren())
            {
                if (child == this)
                    continue;

                if (child is Node3D node3D)
                    return node3D;
            }
        }

        return pai;
    }

    private bool ContemPickup(Node node)
    {
        if (node is genericPickupScript)
            return true;

        foreach (Node child in node.GetChildren())
        {
            if (ContemPickup(child))
                return true;
        }

        return false;
    }

    private bool ContemCharacterBody3D(Node node)
    {
        if (node is CharacterBody3D)
            return true;

        foreach (Node child in node.GetChildren())
        {
            if (ContemCharacterBody3D(child))
                return true;
        }

        return false;
    }

    private void CacheCollisionObjects(Node node)
    {
        if (node is CollisionObject3D collisionObject)
        {
            collisionObjects.Add(collisionObject);
            originalCollisionLayers[collisionObject] = collisionObject.CollisionLayer;
        }

        foreach (Node child in node.GetChildren())
            CacheCollisionObjects(child);
    }

    private void OnFlagChanged(string key, bool value)
    {
        if (key == removalOnFlag ||
            key == showWhenFlag ||
            key == hideWhenFlag)
        {
            UpdateGatekeep();
        }
    }

    private void OnTimeChanged(float minuto)
    {
        if (usarHorario)
            UpdateGatekeep();
    }

    private void OnDayChanged(int dia)
    {
        if (usarDia)
            UpdateGatekeep();
    }

    private void OnDayStateChanged(DayState state)
    {
        if (usarDayState)
            UpdateGatekeep();
    }

    private void UpdateGatekeep()
    {
        if (GameState.Instance.GetFlag(removalOnFlag))
        {
            target?.QueueFree();
            return;
        }

        if (!ConditionsAllowPresence())
        {
            SetVisible(false);
            return;
        }

        SetVisible(true);
    }

    private bool ConditionsAllowPresence()
    {
        if (!CheckFlagConditions())
            return false;

        if (usarHorario && !CheckHorario())
            return false;

        if (usarDia && !CheckDia())
            return false;

        if (usarDayState &&
            TimeState.Instance.CurrentDayState != dayState)
            return false;

        return true;
    }

    private bool CheckFlagConditions()
    {
        if (!string.IsNullOrEmpty(hideWhenFlag) &&
            GameState.Instance.GetFlag(hideWhenFlag))
        {
            return false;
        }

        if (!string.IsNullOrEmpty(showWhenFlag) &&
            !GameState.Instance.GetFlag(showWhenFlag))
        {
            return false;
        }

        return true;
    }

    private bool CheckHorario()
    {
        float minuto = TimeState.Instance.minutoDoDia;

        if (horarioInicio <= horarioFim)
            return minuto >= horarioInicio && minuto <= horarioFim;

        return minuto >= horarioInicio || minuto <= horarioFim;
    }

    private bool CheckDia()
    {
        int dia = TimeState.Instance.diaAtual;

        return dia >= diaMinimo && dia <= diaMaximo;
    }

    private void SetVisible(bool visible)
    {
        if (target == null)
            return;

        SetNodeVisible(target, visible);

        foreach (CollisionObject3D collisionObject in collisionObjects)
        {
            if (!IsInstanceValid(collisionObject))
                continue;

            if (visible)
                collisionObject.CollisionLayer = originalCollisionLayers[collisionObject];
            else
                collisionObject.CollisionLayer = 0;
        }
    }

    private void SetNodeVisible(Node node, bool visible)
    {
        if (node is Node3D node3D)
            node3D.Visible = visible;

        foreach (Node child in node.GetChildren())
            SetNodeVisible(child, visible);
    }
}