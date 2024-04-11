using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenManager : AbstractSceneManager
{
    public StackService<(AbstractPopup, string)> histories = new();

    private void Awake()
    {
        histories = new();
        histories.AutomationPop = false;
        histories.OnPop.AddListener(OpenLast);
    }

    private void OpenLast((AbstractPopup obj, string trackingName) data)
    {
        current = data.obj;
        current?.gameObject.SetActive(true);
        current?.OpenPopup();
        if (data.obj == null || data.trackingName == null)
            return;
    }
    string currentTrackingName;
    public override void Display(string alias, string custom, string trackingName)
    {
        Debug.Log($"Open display : {alias}: {trackingName}");
        histories.Push((current, currentTrackingName));
        current?.ClosePopup(false);
        this.currentTrackingName = trackingName;
        OpenPrefab(alias, custom);
    }

    public override void OnEndClose()
    {
        histories.EndStack();
    }
}