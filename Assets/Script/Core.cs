using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Core : MonoBehaviour
{
    [SerializeField]
    MainCamera mainCamera;
    [SerializeField]
    Figure figure;
    [SerializeField]
    DrawingBoard drawingBoard;
    [SerializeField]
    DrawController drawController;

    CoreUI coreUI;

    private void Awake()
    {
        drawController.LineIsReady += DrawController_LineIsReady;
        mainCamera.CameraDidMoveToFigure += MainCamera_CameraDidMoveToFigure;

        coreUI = GetComponent<CoreUI>();
    }


    // Use this for initialization
    void Start()
    {
        StartLevel();
    }

    public void StartLevel()
    {
        StartCoroutine("StartingAnimations");
    }

    IEnumerator StartingAnimations()
    {
        yield return new WaitForSeconds(2);

        mainCamera.MoveToFigure();
        figure.FadeOut();
    }

    #region Events

    private void DrawController_LineIsReady()
    {
        drawController.IsEnabled = false;
        figure.FadeIn();
        ResultsController resultController = GetComponentInChildren<ResultsController>();
        resultController.CalculateResult();
    }

    private void MainCamera_CameraDidMoveToFigure()
    {
        drawingBoard.FadeIn();
        drawController.IsEnabled = true;
    }

    #endregion
}
