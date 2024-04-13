using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GroupType
{
    NotSet = 0, Jacat = 1, Fredra = 2, Govo = 3, BackOffice = 4
}

public class HiddenItemManager : SerializedMonoBehaviour
{
    [SerializeField] private Dictionary<GroupType, List<HiddenItem>> _allItems;

    [Space(20)]
    public UnityEngine.Events.UnityEvent<HiddenItem> onPickHiddenItem;

    public Dictionary<GroupType, List<HiddenItem>> AllItem => _allItems;

    private void Start()
    {
        foreach (var listType in _allItems.Values)
        {
            foreach(var item in listType)
            {
                item.onClick += OnClickItem;
                item.Init();
            }
        }
    }

    public void OnClickItem(HiddenItem item)
    {
        onPickHiddenItem?.Invoke(item);
    }
}
