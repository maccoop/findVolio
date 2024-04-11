using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


#if UNITY_EDITOR
[UnityEditor.CanEditMultipleObjects]
#endif
public abstract class AbstractPopup : MonoBehaviour
{
    [System.Serializable]
    public enum AnimationPopupType
    {
        None, Zoom, Fade 
    }

    public AnimationPopupType animationType;
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
        switch (animationType)
        {
            case AnimationPopupType.Fade:
                {
                    var canvasGroup = gameObject.AddComponent<CanvasGroup>();
                    canvasGroup.alpha = 0;
                    canvasGroup.DOFade(1, 0.4f).OnComplete(() =>
                    {
                        Destroy(canvasGroup);
                    });
                    break;
                }
            case AnimationPopupType.Zoom:
                {
                    transform.localScale = Vector3.zero;
                    transform.DOScale(1, 0.4f).OnComplete(() =>
                    {

                    });
                    break;
                }
        }
    }

    private void OnCloseScene(bool callback)
    {
        switch (animationType)
        {
            case AnimationPopupType.Fade:
                {
                    var canvasGroup = gameObject.AddComponent<CanvasGroup>();
                    canvasGroup.alpha = 1;
                    canvasGroup.DOFade(0, 0.4f).OnComplete(() =>
                    {
                        Destroy(canvasGroup);
                        EndCloseScene();
                    });
                    break;
                }
            case AnimationPopupType.Zoom:
                {
                    transform.DOScale(0, 0.4f).OnComplete(() =>
                    {
                        EndCloseScene();
                    });
                    break;
                }
            case AnimationPopupType.None:
                {
                    EndCloseScene();
                    break;
                }
        }
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
