using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class User{
    public static (string, float) currentPopupTime, currentScreen;
    public static string currentPopup => currentPopupTime.Item1;
}
