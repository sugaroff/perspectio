using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Figure : MonoBehaviour
{
    public bool fadeout = true;

    FadeInOut fadeInOut;


    private void Awake()
    {
        fadeInOut = GetComponent<FadeInOut>();
    }


    public void FadeOut()
    {
        if (fadeout)
            fadeInOut.FadeOut();
    }

    public void FadeIn()
    {
        fadeInOut.FadeIn();
    }

    public List<Vector3> GetVisibleSidePoligon()
    {
        var points = new List<Vector3>()
        {
            new Vector3(249, 0, 249.5f),
            new Vector3(249, 3, 249.5f),
            new Vector3(251, 3, 249.5f),
            new Vector3(251, 0, 249.5f)
        };

        return points;
    }
}
