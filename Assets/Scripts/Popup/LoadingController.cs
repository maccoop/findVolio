using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadingController : AbstractPopup
{
    public Slider slider;
    public Button btnPlay;
    public GameObject taptoplay;
    private float target;
    public override void OpenPopup(string custom)
    {
        base.OpenPopup(custom); 
        StartCoroutine(Begin());
    }

    private IEnumerator Begin()
    {
        var gameplay = SceneManager.LoadSceneAsync("gameplay", LoadSceneMode.Additive);
        DoValue();
        target = 0.2f;
        var waitUpdate = new WaitForSeconds(0.2f);
        while (!gameplay.isDone)
        {
            target = gameplay.progress;
            yield return waitUpdate;
        }
        yield return waitUpdate; 
        target = 1;
    }

    public void DoValue()
    {
        slider.DOValue(target, 0.3f).OnComplete(() =>
        {
            if (target >= 1)
            {
                slider.DOValue(1, 0.3f).OnComplete(() =>
                {
                    slider.gameObject.SetActive(false);
                    btnPlay.onClick.AddListener(onClickToPlay);
                    taptoplay.gameObject.SetActive(true);
                });
            }
            else
            {
                DoValue();
            }
        });
    }

    public void onClickToPlay()
    {
        PopupSceneManagerController.CloseCurrentPopup();
        SceneManager.UnloadSceneAsync("home");
        User.AddTracking(ActionType.open_screen, "gameplay");
    }
}
