using System;
using LitJson;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public partial class UserData
{
    public int session;
    public bool policy;
    public bool isSetFirstDay = false;
    public DateTime? secondDayOpenApp;
    public UserData()
    {
        Init();
    }

    partial void Init();
}


public partial class User
{
    const string userDataPKey = "USER_DATA";

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void Load()
    {
        var textData = PlayerPrefs.GetString(userDataPKey, string.Empty);

        if (!string.IsNullOrEmpty(textData))
        {
            data = JsonMapper.ToObject<UserData>(textData);
        }
        else
        {
            data = new UserData();
            Save();
        }
    }

    public static void RemoveAll()
    {
        data = new UserData();
    }

    private static UserData data;

    public static void Save()
    {
        var textData = DataToString();

        PlayerPrefs.SetString(userDataPKey, textData);
    }

    public static string DataToString() { return JsonMapper.ToJson(data); }


    public static int PlayerSession
    {
        get
        {
            return data.session;
        }
        internal set
        {
            data.session = value;
            Save();
        }
    }

    public static bool Policy
    {
        get
        {
            return data.policy;
        }
        set
        {
            data.policy = value;
            Save();
        }
    }


    #region Subscribe
    private static bool isSubscribe;

    public static bool IsSubscribe
    {
        get
        {
            return isSubscribe;
        }
        set
        {
            isSubscribe = value;
        }
    }

    public static void SetSubscribe()
    {
        IsSubscribe = true;
    }
    #endregion

    public static void CheckSetDays()
    {
        if (data.isSetFirstDay) return;
        data.isSetFirstDay = true;

        var now = DateTime.Now;
        data.secondDayOpenApp = new DateTime(now.Year, now.Month, now.Day).AddDays(1);
        Save();
    }


    public static DateTime GetSecondDay()
    {
        if (!data.secondDayOpenApp.HasValue)
        {
            CheckSetDays();
        }
        return data.secondDayOpenApp.Value;
    }
    public static void AddTracking(ActionType type, string alias, int number = 0, string custom = null, bool logTracking = false)
    {
        var condition = new ActionCondition(type, alias, number);
        AddTracking(condition, custom, logTracking);
    }

    public static void AddTracking(ActionCondition condition, string custom = null, bool logTracking = false)
    {
        GameCallAction(condition, custom);
        var str = condition.GetCondition();
        str = str.Replace("-", "_");
        InGameAction.Instance.ActionInGame(str, custom);
    }

    private static void GameCallAction(ActionCondition condition, string custom)
    {
        switch (condition.Type)
        {
            case ActionType.find_object:
                {
                    User.Finder(condition.Alias);
                    var story = StoryConfig.Instance.GetStory(condition.Alias);
                    if (story != null)
                    {
                        AddTracking(ActionType.find_story, condition.Alias);
                    }
                    break;
                }
            case ActionType.unlock_map:
                {
                    User.UnlockMap(custom);
                    break;
                }
        }
    }
}

