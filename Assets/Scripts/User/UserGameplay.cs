using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public partial class User
{
    private static Dictionary<string, int> finder = new();
    private static UnityEvent<string> onFinded = new();

    public static void Finder(string id)
    {
        var groupid = id.Split("_")[1];
        if (!finder.ContainsKey(groupid))
        {
            finder.Add(groupid, 0);
        }
        finder[groupid]++;
        onFinded.Invoke(groupid);
    }

    public static int AmountFinded(string id)
    {
        if (!finder.ContainsKey(id))
        {
            finder.Add(id, 0);
        }
        return finder[id];
    }

    public static void AddListenerOnFinded(UnityAction<string> onNext)
    {
        onFinded.AddListener(onNext);
    }
}
