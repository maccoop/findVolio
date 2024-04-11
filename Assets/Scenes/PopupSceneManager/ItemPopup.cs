
#if UNITY_EDITOR
using Sirenix.OdinInspector;

[UnityEditor.CanEditMultipleObjects]
#endif
public class ItemPopup : AbstractPopup
{
    public CloseType closeType;
    [ShowIf("IsCloseByTime")]
    public float timeClose;
    public bool IsCloseByTime => closeType == CloseType.Time;
    public override void OpenPopup(string custom)
    {
        base.OpenPopup(custom);
        switch (closeType)
        {
            case CloseType.Time:
                {
                    Invoke(nameof(ClosePopup), timeClose);
                    break;
                }
        }
    }
}

[System.Serializable]
public enum CloseType
{
    Scripts, Time
}
