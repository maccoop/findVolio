using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
[UnityEditor.CanEditMultipleObjects]
#endif
public abstract class AbstractPopup : MonoBehaviour
{
    public bool disableInGameUI = true;
    public bool showBG = true;
    public string Alias => gameObject.name;
    private string custom;
    internal AbstractSceneManager manager;

    public virtual void OpenPopup(string custom)
    {
        this.custom = custom;
        OnOpenPopup();
    }

    public void OpenPopup()
    {
        OpenPopup(this.custom);
    }

    public virtual void ClosePopup()
    {
        OnCloseScene(true);
    }

    public virtual void ClosePopup(bool callback)
    {
        OnCloseScene(callback);
    }

    private void OnOpenPopup()
    {
    }

    private void OnCloseScene(bool callback)
    {
        EndCloseScene();
        if (callback)
        {
            manager.OnEndClose();
        }
    }

    void EndCloseScene()
    {
        gameObject.SetActive(false);
    }
}
