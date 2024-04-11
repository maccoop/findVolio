using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "FinderConfig", menuName = "config/FinderConfig")]
public class FinderConfig : SingletonScriptableObject<FinderConfig>
{
    public List<FinderItem> items;

    public FinderItem Get(string id) => items.Find(x => x.Id.Equals(id));
}

[System.Serializable]
public class FinderItem: AbstractData
{
    public int amountItem;
}
