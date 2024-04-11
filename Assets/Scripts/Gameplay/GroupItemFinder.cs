using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GroupItemFinder : MonoBehaviour
{
    public string id;
    public Image icon;
    public Slider slider;

    FinderItem data;
    void Start()
    {
        data = FinderConfig.Instance.Get(id);
        icon.sprite = data.Thumbnail;
        {
            slider.maxValue = data.amountItem;
            slider.minValue = 0;
            slider.value = User.AmountFinded(id);
        }
        User.AddListenerOnFinded(OnUpdate);
    }

    private void OnUpdate(string id)
    {
        if (this.id.Equals(id))
        {
            slider.DOKill();
            slider.DOValue(User.AmountFinded(id), 0.3f).OnComplete(() =>
            {
                if (User.AmountFinded(id) >= FinderConfig.Instance.Get(id).amountItem)
                {
                    User.AddTracking(ActionType.unlock_map, "", 0, id);
                }
            });
        }
    }
}
