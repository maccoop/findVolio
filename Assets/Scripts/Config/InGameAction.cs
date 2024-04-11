using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "InGameAction", menuName = "config/InGameAction")]
public class InGameAction : SingletonScriptableObject<InGameAction>
{
    public List<ActionData> actions;
    [Space]
    [Title("Search:")]
    public List<ActionData> result;

    [Button]
    public void FindByCondition(string key)
    {
        result = actions.FindAll(x => x.Condition.GetCondition().Contains(key));
    }
    [Button]
    public void FindByPopup(string key)
    {
        result = actions.FindAll(x => x.Results != null && x.Results.Length > 0 && x.Results[0].popupUp.Contains(key));
    }

    [ContextMenu("CreateCondition")]
    public void UpdateCondition()
    {
        foreach (var e in actions)
        {
            e.Condition.GenerateCondition();
        }
    }

    public void ActionInGame(string condition, string custom)
    {
        var missons = actions.FindAll(x => x.Condition.GetCondition().Equals(condition));
        if (missons != null)
        {
            foreach (var quest in missons)
            {
                foreach (var e in quest.Results)
                {
                    var cd = e;
                    if (string.IsNullOrEmpty(e.Custom))
                    {
                        cd.Custom = custom;
                    }
                    if (e.Custom.Equals("IMPORTANT"))
                    {
                        cd.Custom += custom;
                    }
                    cd.CallAction();
                }
            }
        }
    }
}
[System.Serializable]
public struct ActionData
{
    [SerializeField, ReadOnly, OnStateUpdate("ChangeTitle"), GUIColor("GetColor"), HideLabel] string title;
    [SerializeField, TabGroup("Editor")] ActionCondition condition;
    [SerializeField, TabGroup("Editor")] ActionResult[] results;

    public ActionCondition Condition { get => condition; set => condition = value; }
    public ActionResult[] Results => results;


    public void ChangeTitle()
    {
        title = condition.GetCondition();
    }
    public Color GetColor()
    {
        return Color.green;
    }
}

[System.Serializable]
public class ActionCondition : ICondition
{
    [SerializeField, OnValueChanged("GenerateCondition")] ActionType type;
    [SerializeField, OnValueChanged("GenerateCondition")] string alias;
    [SerializeField, OnValueChanged("GenerateCondition")] int number;
    [SerializeField, ReadOnly] string condition;

    public ActionCondition(ActionType type, string alias, int number)
    {
        this.type = type;
        this.alias = alias;
        this.number = number;
        GenerateCondition();
    }

    public ActionType Type { get => type; }
    public int Number { get => number; }
    public string Alias { get => alias; }

    public void GenerateCondition()
    {
        condition = GetCondition();
    }

    public string GetCondition()
    {
        if (number != 0)
            return $"{type}_{Alias}_{number}";
        return $"{type}_{Alias}";
    }
}

[System.Serializable]
public struct ActionResult
{
    public Type type;
    [ValueDropdown("GetAllPopup")]
    public string popupUp;
    [HideIf("IsPopup")]
    public string alias;
    public string custom;
    public bool IsPopup => type == Type.Popup;
    public string Alias => IsPopup ? popupUp : alias;

    public string Custom
    {
        get
        {
                return custom;
        }
        set
        {
                custom = value;
        }
    }

    [System.Serializable]
    public enum Type
    {
        ReceiveItem = 0, Popup = 1
    }

    public void CallAction()
    {
        switch (type)
        {
            case Type.ReceiveItem:
                {
                    break;
                }
            case Type.Popup:
                {
                    PopupSceneManagerController.ShowPopup(Alias, Custom, null);
                    break;
                }
        }
    }

    public List<string> GetAllPopup()
    {
        var abstractPopups = Resources.LoadAll<AbstractPopup>("");
        return abstractPopups.Select(x => x.name).ToList();
    }
}

[System.Serializable]
public enum ActionType
{
    find_object = 1,
    open_screen = 2,
    open_dialog = 3,
    unlock_map = 4,
    reply_gameplay = 5,
    close_dialog = 6,
    pause = 7,
    find_story = 8
}

[System.Serializable]
public enum QuestType
{
    find_object = 1
}
