using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ClipperLib;
using System;

public class ResultsController : MonoBehaviour {

    const int scale = 1000;
    float z;
    

	public void CalculateResult()
    {
        // Figure
        GameObject figureGO = GameObject.FindGameObjectWithTag(Tags.Figure);
        Figure figure = figureGO.GetComponent<Figure>();

        List<Vector3> figureSidePolygon = figure.GetVisibleSidePoligon();
        List<IntPoint> figureIntPoints = ConvertVectors3ToIntPoints(figureSidePolygon);

        // Line
        GameObject drawControllerGO = GameObject.FindGameObjectWithTag(Tags.DrawController);
        DrawController drawController = drawControllerGO.GetComponent<DrawController>();
        LineRenderer line = drawController.Line;
        Vector3[] lineVectorsArray = new Vector3[line.positionCount];
        line.GetPositions(lineVectorsArray);
        List<IntPoint> lineIntPoints = ConvertVectors3ToIntPoints(lineVectorsArray);

        Clipper c = new Clipper();
        c.AddPaths(new List<List<IntPoint>>() { figureIntPoints }, PolyType.ptSubject, true);
        c.AddPaths(new List<List<IntPoint>>() { lineIntPoints }, PolyType.ptClip, true);

        var resultPolygons = new List<List<IntPoint>>();

        bool succeeded = c.Execute(ClipType.ctIntersection, resultPolygons);

        Debug.Log("successed: " + succeeded);

        if (succeeded)
        {
            foreach (var polygon in resultPolygons)
            {
                GameObject go = new GameObject();
                PolygonRenderer polRenderer = go.AddComponent<PolygonRenderer>();
                List<Vector3> points = ConvertIntPointsToVectors3(polygon);
                points.Reverse();
                polRenderer.Points = points;
                polRenderer.Mat = Resources.Load<Material>("Polygon");
            }
        }
    }



    List<IntPoint> ConvertVectors3ToIntPoints(IEnumerable<Vector3> vectorsList)
    {
        List<IntPoint> intPointsList = new List<IntPoint>();

        foreach (var vector in vectorsList)
        {
            intPointsList.Add(new IntPoint(
                Convert.ToInt64(vector.x * scale),
                Convert.ToInt64(vector.y * scale)));

            z = vector.z;
        }

        return intPointsList;
    }

    List<Vector3> ConvertIntPointsToVectors3(IEnumerable<IntPoint> intPointsList)
    {
        List<Vector3> vectorsList = new List<Vector3>();

        foreach (var intPoint in intPointsList)
        {
            vectorsList.Add(new Vector3(
                intPoint.X / (float)scale,
                intPoint.Y / (float)scale,
                z));
        }

        return vectorsList;
    }
}
