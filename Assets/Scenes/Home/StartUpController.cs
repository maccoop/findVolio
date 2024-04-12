using UnityEngine;

public class StartUpController : MonoBehaviour
{
    void Awake()
    {
        User.AddTracking(ActionType.open_dialog, "loading");
    }
}
