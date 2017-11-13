using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DrawUI : MonoBehaviour
{
    [SerializeField]
    private CanvasGroup drawingUI;
    [SerializeField]
    private Button redrawButton;
    [SerializeField]
    private Button readyButton;

    // Use this for initialization
    void Start()
    {
        HideUI();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void ShowUI()
    {
        drawingUI.alpha = 1;
        drawingUI.interactable = true;
    }

    public void HideUI()
    {
        drawingUI.alpha = 0;
        drawingUI.interactable = false;
    }
}
