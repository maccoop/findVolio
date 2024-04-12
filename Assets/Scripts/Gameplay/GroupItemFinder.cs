using DG.Tweening;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GroupItemFinder : MonoBehaviour
{
    [ValueDropdown("GetGroupValue")]
    public string id;
    public Image icon;
    public Image slider;
    public TMP_Text amount;

    private IEnumerable<string> GetGroupValue()
    {
        return FinderConfig.Instance.items.Select(x => x.Id);
    }

    FinderItem data;
    void Start()
    {
        data = FinderConfig.Instance.Get(id);
        //icon.sprite = data.Thumbnail;
        slider.fillAmount = User.AmountFinded(id) / data.amountItem;
        amount.text = User.AmountFinded(id) + "/" + data.amountItem;
        User.AddListenerOnFinded(OnUpdate);
    }

    private void OnUpdate(string id)
    {
        if (this.id.Equals(id))
        {
            amount.text = User.AmountFinded(id) + "/" + data.amountItem;
            slider.DOKill();
            slider.DOFillAmount(User.AmountFinded(id) / data.amountItem, 0.3f).OnComplete(() =>
            {
                if (User.AmountFinded(id) >= FinderConfig.Instance.Get(id).amountItem)
                {
                    User.AddTracking(ActionType.unlock_map, "", 0, id);
                }
            });
        }
    }
}
