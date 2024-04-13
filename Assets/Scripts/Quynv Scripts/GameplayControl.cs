using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameplayControl : MMSingleton<GameplayControl>
{
    public void OnPickHiddenItem(HiddenItem item)
    {
        if(SpecialItemsInfo.Instance.IsSpecialItem(item, out var itemInfo))
        {
            // show popup
        }
    }
}
