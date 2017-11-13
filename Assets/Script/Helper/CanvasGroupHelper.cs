using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class CanvasGroupHelper
{

    public static void Show(this CanvasGroup canvasGroup)
    {
        canvasGroup.alpha = 1;
        canvasGroup.interactable = true;
    }

    public static void Hide(this CanvasGroup canvasGroup)
    {
        canvasGroup.alpha = 0;
        canvasGroup.interactable = false;
    }
}
