using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class User
{
    private static List<string> groupUnlock;

    public static void UnlockMap(string id)
    {
        groupUnlock.Add(id);
    }

    public static bool GetUnlockMap(string id) => groupUnlock.Contains(id);
}
