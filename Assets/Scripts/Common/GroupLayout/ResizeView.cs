using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

[ExecuteInEditMode]
public class ResizeView : UIBehaviour
{
    private ScreenOrientation deviceOrientation;
    private RectTransform rectTransform;

    protected override void Start()
    {
        base.Start();
        rectTransform = GetComponent<RectTransform>();
    }

    private void Update()
    {
        if(Screen.orientation != deviceOrientation)
        {
            deviceOrientation = Screen.orientation;
            switch (Screen.orientation)
            {
                case ScreenOrientation.LandscapeLeft:
                case ScreenOrientation.LandscapeRight:
                    {
                        rectTransform.anchorMax = Vector2.zero;
                        rectTransform.anchorMin = Vector2.zero;
                        rectTransform.sizeDelta = new Vector2(887, 228);
                        break;
                    }
                case ScreenOrientation.Portrait:
                    {
                        rectTransform.anchorMax = Vector2.right;
                        rectTransform.anchorMin = Vector2.zero;
                        rectTransform.sizeDelta = new Vector2(0, 228);
                        break;
                    }
            }
        }
}
}
