using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCamera : MonoBehaviour
{
    Animator animator;

    public event Action CameraDidMoveToFigure;


    private void Awake()
    {
        animator = GetComponent<Animator>();
#if FAST_EXECUTION
        animator.speed = 100;
#endif
    }


    public void MoveToFigure()
    {
        animator.SetTrigger("MoveToObject");
    }

    void CameraMovedToFigure()
    {
        if (CameraDidMoveToFigure != null)
            CameraDidMoveToFigure();
    }
}

