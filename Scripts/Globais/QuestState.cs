using Godot;
using System.Collections.Generic;

public partial class QuestState : Node
{
    public static QuestState Instance { get; private set; }

    public List<QuestResource> Quests { get; private set; } = new();

    [Signal]
    public delegate void QuestCompletedEventHandler(QuestResource quest);

    public override void _EnterTree()
    {
        if (Instance != null && Instance != this)
        {
            QueueFree();
            return;
        }

        Instance = this;
    }

    public override void _Ready()
    {
        GameState.Instance.FlagChanged += OnFlagChanged;
    }

    public override void _ExitTree()
    {
        if (GameState.Instance != null)
            GameState.Instance.FlagChanged -= OnFlagChanged;
    }

    private void OnFlagChanged(string key, bool value)
    {
        UpdateQuest();
    }

    public void AddQuest(string questId)
    {
        if (string.IsNullOrEmpty(questId))
            return;

        QuestResource questExistente = GetQuest(questId);

        if (questExistente != null)
            return;

        string caminho = $"res://Assets/Quests/{questId}.tres";

        if (!ResourceLoader.Exists(caminho))
        {
            GD.PrintErr($"Quest não encontrada: {caminho}");
            return;
        }

        QuestResource quest = ResourceLoader.Load<QuestResource>(caminho);

        if (quest == null)
        {
            GD.PrintErr($"Não foi possível carregar a quest: {caminho}");
            return;
        }

        quest.Iniciar();
        Quests.Add(quest);
        quest.Atualizar(GameState.Instance);
    }

    public void RemoveQuest(QuestResource quest)
    {
        if (quest == null)
            return;

        Quests.Remove(quest);
    }

    public QuestResource GetQuest(string id)
    {
        foreach (QuestResource quest in Quests)
        {
            if (quest.Id == id)
                return quest;
        }

        return null;
    }

    private void UpdateQuest()
    {
        foreach (QuestResource quest in Quests)
        {
            if (quest.Concluida)
                continue;

            quest.Atualizar(GameState.Instance);

            if (quest.Concluida)
                EmitSignal(SignalName.QuestCompleted, quest);
        }
    }
}