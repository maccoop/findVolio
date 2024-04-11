using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartUpController : MonoBehaviour
{
    // Start is called before the first frame update
    void Awake()
    {
        User.AddTracking(ActionType.open_dialog, "loading");
    }

    IEnumerator Start()
    {
        yield break;
        var gameplay = SceneManager.LoadSceneAsync("gameplay", LoadSceneMode.Additive);
        var waitUpdate = new WaitForFixedUpdate();
        while (!gameplay.isDone)
        {
            yield return waitUpdate;
        }
        yield return new WaitForSeconds(0.1f);
        PopupSceneManagerController.CloseCurrentPopup();
        User.AddTracking(ActionType.open_screen, "gameplay");
    }
}
