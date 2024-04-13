using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ItemInfo
{
    public string itemName;
    public Sprite spr;
    public string title;
    public string description;
}

[CreateAssetMenu(fileName = "SpecialItemInfo", menuName = "Gameplay/Special Item Info")]
public class SpecialItemsInfo : SingletonScriptableObject<SpecialItemsInfo>
{
    [SerializeField] private List<ItemInfo> items;

    public bool IsSpecialItem(HiddenItem item, out ItemInfo info)
    {
        info = items.Find(x => x.itemName.Equals(item.ItemName));
        return info != null;
    }
}
