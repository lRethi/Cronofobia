using Godot;

using System.Collections.Generic;

public partial class QuestState : Node
{
    public static QuestState Instance { get; private set; }

    public Dictionary<string, QuestResource> Quests { get; private set; } = new();

    [Signal]
    public delegate void QuestCompletedEventHandler(QuestResource quest);

    [Signal]
    public delegate void QuestsChangedEventHandler();

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

        if (Quests.ContainsKey(questId))
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

        Quests.Add(questId, quest);

        quest.Atualizar(GameState.Instance);

        EmitSignal(SignalName.QuestsChanged);
    }

    public void RemoveQuest(QuestResource quest)
    {
        if (quest == null)
            return;

        if (Quests.Remove(quest.Id))
            EmitSignal(SignalName.QuestsChanged);
    }

    public QuestResource GetQuest(string id)
    {
        if (string.IsNullOrEmpty(id))
            return null;

        return Quests.TryGetValue(id, out QuestResource quest)
            ? quest
            : null;
    }

    public IEnumerable<QuestResource> GetQuestsAtivas()
    {
        foreach (QuestResource quest in Quests.Values)
        {
            if (quest.Ativa && !quest.Concluida)
                yield return quest;
        }
    }

    private void UpdateQuest()
    {
        foreach (QuestResource quest in Quests.Values)
        {
            if (quest.Concluida)
                continue;

            bool estavaAtiva = quest.Ativa;
            bool estavaConcluida = quest.Concluida;

            quest.Atualizar(GameState.Instance);

            if (!estavaConcluida && quest.Concluida)
                EmitSignal(SignalName.QuestCompleted, quest);

            if (estavaAtiva != quest.Ativa || estavaConcluida != quest.Concluida)
                EmitSignal(SignalName.QuestsChanged);
        }
    }
    public void Resetar()
    {
        Quests.Clear();
        EmitSignal(SignalName.QuestsChanged);
    } 
}