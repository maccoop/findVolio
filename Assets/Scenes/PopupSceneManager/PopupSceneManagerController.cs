using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public partial class PopupSceneManagerController : MonoBehaviour
{
    public const string POPUPSCENEMANAGER_SCENE_NAME = "PopupSceneManager";

    public AbstractSceneManager screen, popup, importal;
    private static PopupSceneManagerController instance;
    private static bool isInit;

    private static async Task LoadScene()
    {
        if (!isInit)
            SceneManager.LoadScene(POPUPSCENEMANAGER_SCENE_NAME, LoadSceneMode.Additive);
        isInit = true;
        while (instance == null)
        {
            await Task.Delay(100);
        }
        return;
    }

    public static async void ShowPopup(string alias, string custom, string trackingName)
    {
        if (instance == null)
        {
            await LoadScene();
        }
        if (alias.Contains("screen"))
        {
            instance.screen.Display(alias, custom, trackingName);
        }
        else if (custom.Contains("IMPORTAL"))
        {
            instance.importal.Display(alias, custom.Replace("IMPORTAL", ""), trackingName);
        }
        else
        {
            instance.popup.Display(alias, custom, trackingName);
        }
    }

    public static void CloseCurrentPopup()
    {
        instance?.popup.current?.ClosePopup();
    }

    private void Awake()
    {
        instance = this;
    }

    private void OnDestroy()
    {
        instance = null;
        isInit = false;
    }
}

