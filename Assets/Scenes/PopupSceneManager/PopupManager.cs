using DG.Tweening;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PopupManager : AbstractSceneManager
{
    public CanvasGroup bg;
    public QueueService<(string, string)> queue = new();

    private void Awake()
    {
        queue.OnDequeue.AddListener(OnDequeu);
        bg.gameObject.SetActive(false);
    }

    private void OnDequeu((string alias, string custom) value)
    {
        OpenPrefab(value.alias, value.custom);
        if (current != null && current.showBG)
        {
            bg.DOKill();
            bg.DOFade(1, 0.3f);
            bg.gameObject.SetActive(true);
        }
    }

    public override void Display(string alias, string custom, string trackingName)
    {
        if (current != null && (alias.Equals(current.name) || queue.Contains((alias, custom))))
            return;
        queue.AddQueue((alias, custom));
    }
    public override void OnEndClose()
    {
        if (current != null && current.showBG)
        {
            bg.DOFade(0, 0.3f).OnComplete(() =>
            {
                bg.gameObject.SetActive(false);
            });
        }
        current = null;
        queue.EndQueue();
    }
}