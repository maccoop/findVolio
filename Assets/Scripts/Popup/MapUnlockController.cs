using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MapUnlockController : AbstractPopup
{
    public List<MapHidden> hiddens;

    public override void OpenPopup(string custom)
    {
        base.OpenPopup(custom);
        var hidden = hiddens.Find(x => x.id.Equals(custom));
        hidden.prefab.gameObject.SetActive(false); 
        CheckComplete();
    }

    private void CheckComplete()
    {
        foreach(var e in hiddens)
        {
            if (e.prefab.activeSelf)
            {
                return;
            }
        }
        StartCoroutine(CoComplete());
    }

    private IEnumerator CoComplete()
    {
        yield return new WaitForSeconds(1);
        PopupSceneManagerController.CloseCurrentPopup();
        User.AddTracking(ActionType.open_screen, "win", 0);
    }
}

[System.Serializable]
public class MapHidden
{
    [ValueDropdown("GetValue")]
    public string id;
    public GameObject prefab;

    public IEnumerable<string> GetValue() => FinderConfig.Instance.items.Select(x => x.Id);
}
