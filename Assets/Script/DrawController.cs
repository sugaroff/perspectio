﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class DrawController : MonoBehaviour
{
    public DrawUI drawUI;
    public DrawingBoard DrawingBoard;
    bool isMousePressed;
    bool isEnabled = false;

    public event Action LineIsReady;


    public bool IsEnabled
    {
        get { return isEnabled; }
        set
        {
            isEnabled = value;
            if (isEnabled)
                drawUI.ShowUI();
            else
                drawUI.HideUI();
        }
    }
    
    public LineRenderer Line { get; private set; }


    #region Unity Events

    private void Awake()
    {
        GameObject line = new GameObject();
        line.transform.parent = transform;

        Line = line.AddComponent<LineRenderer>();
        Line.material = new Material(Shader.Find("Standard"));
        Line.material.color = Color.red;
        Line.startWidth = Line.endWidth = 0.05f;
        //Line.startColor = Line.endColor = Color.red;
        Line.positionCount = 0;
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject())
        {
            Vector3 hitPoint;
            if (TryGetPoint(out hitPoint) && isEnabled)
            {
                StartDraw(hitPoint);
            }
        }
        if (Input.GetMouseButtonUp(0))
        {
            EndDraw();
        }
        if (isMousePressed)
        {
            Vector3 hitPoint;
            if (TryGetPoint(out hitPoint))
            {
                UpdateDraw(hitPoint);
            }
        }
    }

    #endregion


    void StartDraw(Vector3 point)
    {
        isMousePressed = true;

        if (Line.positionCount == 0)
        {
            Line.positionCount = 2;
            Line.SetPosition(0, point);
            Line.SetPosition(1, point);
        }
        else
        {
            Line.positionCount++;
            Line.SetPosition(Line.positionCount - 1, point);
         }
    }

    void EndDraw()
    {
        isMousePressed = false;

    }

    void UpdateDraw(Vector3 point)
    {
        Line.SetPosition(Line.positionCount - 1, point);
    }


    bool TryGetPoint(out Vector3 point)
    {
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit))
        {
            GameObject hitObject = hit.transform.gameObject;
            
            if (hitObject.GetComponent<DrawingBoard>() == DrawingBoard)
            {
                point = hit.point;
                return true;
            }
        }

        point = Vector3.zero;
        return false;
    }


    #region UI Events

    public void Redraw()
    {
        Line.positionCount = 0;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void Ready()
    {
        if (LineIsReady != null)
            LineIsReady();
    }

    #endregion
}

