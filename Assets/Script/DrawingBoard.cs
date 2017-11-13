using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawingBoard : MonoBehaviour {

    FadeInOut fadeInOut;


    private void Awake()
    {
        fadeInOut = GetComponent<FadeInOut>();
        Renderer renderer = GetComponent<Renderer>();
        renderer.enabled = false;
    }


    public void FadeOut()
    {
        fadeInOut.FadeOut();
    }

    public void FadeIn()
    {
        fadeInOut.FadeIn();
    }
}
