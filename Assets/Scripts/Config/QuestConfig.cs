using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using System.Linq;

[CreateAssetMenu(fileName = "QuestConfig", menuName = "config/QuestConfig")]
public class QuestConfig : SingletonScriptableObject<QuestConfig>
{
    public const int TotalQuest = 4;
    public int secondTimeDailyQuest;
    [TableList]
    [SerializeField] private List<QuestData> quests;
    private List<(string, int, int)> resultNull = new();


    [ContextMenu("CreateCondition")]
    public void UpdateCondition()
    {
        foreach (var e in quests)
        {
            e.Condition.GenerateCondition();
        }
    }
}

[System.Serializable]
public class QuestData : AbstractData
{
    [SerializeField] QuestCondition condition;
    [SerializeField] SingleCondition<int> amount;
    [SerializeField] Reward rewards;
    [SerializeField] ConditionArray<int> indexRequire;

    public QuestCondition Condition { get => condition; set => condition = value; }
    public Reward Rewards => rewards;
    public int Amount => amount.require;
    public SingleCondition<int> AmountCondition => amount;
}

[System.Serializable]
public class QuestCondition : ICondition
{
    [SerializeField, OnValueChanged("GenerateCondition")] QuestType type;
    [SerializeField, OnValueChanged("GenerateCondition")] string alias;
    [SerializeField, OnValueChanged("GenerateCondition")] int number;
    [SerializeField, ReadOnly] string condition;

    public QuestType Type { get => type; }
    public QuestCondition(QuestType type, string alias, int number)
    {
        this.type = type;
        this.alias = alias;
        this.number = number;
    }


    public void GenerateCondition()
    {
        condition = GetCondition();
    }

    public string GetCondition()
    {
        if (number != 0)
            return $"{Type}_{alias}_{number}";
        return $"{Type}_{alias}";
    }
}
