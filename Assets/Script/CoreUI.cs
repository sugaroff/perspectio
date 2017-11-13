using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoreUI : MonoBehaviour
{
    [SerializeField]
    CanvasGroup finishLevelUI;

    public void MakefinishLevelUIVisible(bool visible)
    {
        if (visible)
            finishLevelUI.Show();
        else
            finishLevelUI.Hide();
    }


}
