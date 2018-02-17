using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class PolygonRenderer : MonoBehaviour
{
    public List<Vector3> Points;
    public Material Mat;

    private struct PolyPoint
    {
        public int PointNext;
        public int PointPrev;

        public int EarNext;
        public int EarPrev;

        public int ReflNext;
        public int ReflPrev;

        public bool isEar;

    }

    private List<Vector3> m_TriPointList;
    private int Pointcount;
    private PolyPoint[] PolyPointList;

    private Mesh m_Mesh;
    private MeshFilter m_MeshFilter;
    private MeshRenderer m_MeshRenderer;
    private Vector2[] m_Uv;

    void Start()
    {
        Pointcount = Points.Count;
        PolyPointList = new PolyPoint[Pointcount + 1];
        m_TriPointList = new List<Vector3>();

        FillLists();

        Triangulate();

        DrawPolygon();
    }

    private void FillLists()
    {
        /*
		 * three doubly linked lists (points list,reflective points list, ears list) are
		 * maintained in the "PolyPointList" arry.
		 * points list is a cyclic list while other two arent.
		 * 0 index of the Point list is kept only for entering the lists
		 * -1 means undefined link
		 */
        PolyPoint p = new PolyPoint();

        PolyPointList[0] = p;
        PolyPointList[0].PointNext = 1;
        PolyPointList[0].PointPrev = -1;
        PolyPointList[0].EarNext = -1;
        PolyPointList[0].EarPrev = -1;
        PolyPointList[0].ReflNext = -1;
        PolyPointList[0].ReflPrev = -1;
        PolyPointList[0].isEar = false;

        int T_Reflective = -1;
        int T_Convex = -1;

        for (int i = 1; i <= Pointcount; i++)
        {
            PolyPointList[i] = p;

            if (i == 1)
                PolyPointList[i].PointPrev = Pointcount;
            else
                PolyPointList[i].PointPrev = i - 1;

            PolyPointList[i].PointNext = (i % Pointcount) + 1;

            if (isReflective(i))
            {
                PolyPointList[i].ReflPrev = T_Reflective;

                if (T_Reflective == -1)
                {
                    PolyPointList[0].ReflNext = i;
                }
                else
                    PolyPointList[T_Reflective].ReflNext = i;

                T_Reflective = i;
                PolyPointList[i].ReflNext = -1;

                PolyPointList[i].EarPrev = -1;
                PolyPointList[i].EarNext = -1;
            }
            else
            {
                PolyPointList[i].ReflPrev = -1;
                PolyPointList[i].ReflNext = -1;
                PolyPointList[i].isEar = true;

                PolyPointList[i].EarPrev = T_Convex;

                if (T_Convex == -1)
                {
                    PolyPointList[0].EarNext = i;
                }
                else
                    PolyPointList[T_Convex].EarNext = i;

                T_Convex = i;

                PolyPointList[i].EarNext = -1;
            }
        }
        
        int Con = PolyPointList[0].EarNext;

        while (Con != -1)
        {
            if (!isCleanEar(Con))
            {
                RemoveEar(Con);
            }
            Con = PolyPointList[Con].EarNext;
        }
    }


    /*
	 * "Ear Clipping" is used for
	 * Polygon triangulation
	 */
    private void Triangulate()
    {
        int i;

        while (Pointcount > 3)
        {
            /*
			 * The Two-Ears Theorem: "Except for triangles every 
			 * simple ploygon has at least two non-overlapping ears"
			 * so there i will always have a value
			 */
            i = PolyPointList[0].EarNext;

            int PrevP = PolyPointList[i].PointPrev;
            int NextP = PolyPointList[i].PointNext;

            m_TriPointList.Add(new Vector3(PrevP, i, NextP));

            RemoveEar(i);
            RemoveP(i);

            if (!isReflective(PrevP))
            {
                if (isCleanEar(PrevP))
                {
                    if (!PolyPointList[PrevP].isEar)
                    {
                        AddEar(PrevP);
                    }
                }
                else
                {
                    if (PolyPointList[PrevP].isEar)
                    {
                        RemoveEar(PrevP);
                    }
                }
            }

            if (!isReflective(NextP))
            {
                if (isCleanEar(NextP))
                {
                    if (!PolyPointList[NextP].isEar)
                    {
                        AddEar(NextP);
                    }
                }
                else
                {
                    if (PolyPointList[NextP].isEar)
                    {
                        RemoveEar(NextP);
                    }
                }
            }
        }

        int y = PolyPointList[0].PointNext;
        int x = PolyPointList[y].PointPrev;
        int z = PolyPointList[y].PointNext;

        m_TriPointList.Add(new Vector3(x, y, z));
    }



    private void DrawPolygon()
    {
        m_MeshFilter = (MeshFilter)GetComponent(typeof(MeshFilter));
        m_MeshRenderer = (MeshRenderer)GetComponent(typeof(MeshRenderer));
        m_MeshRenderer.GetComponent<Renderer>().material = Mat;
        m_Mesh = m_MeshFilter.mesh;

        int vertex_count = (Points.Count * 2) - 1;
        int triangle_count = m_TriPointList.Count * 3 * 2;

        /*
		 * Mesh vertices
		 */
        Vector3[] vertices = new Vector3[vertex_count];

        for (int i = 0; i < Points.Count; i++)
        {
            vertices[i] = vertices[vertex_count - 1 - i] = Points[i];
        }

        m_Mesh.vertices = vertices;

        /*
		 * Mesh trangles
		 */


        int[] tri = new int[triangle_count];

        for (int i = 0, j = 0; i < m_TriPointList.Count; i++, j += 3)
        {
            tri[j] = tri[triangle_count - 1 - j] = (int)(m_TriPointList[i].x - 1);
            tri[j + 1] = tri[triangle_count - 2 - j] = (int)(m_TriPointList[i].y - 1);
            tri[j + 2] = tri[triangle_count - 3 - j] = (int)(m_TriPointList[i].z - 1);
        }

        m_Mesh.triangles = tri;

        /*
		 * Mesh noramals
		 */
        Vector3[] normals = new Vector3[vertex_count];

        for (int i = 0; i < vertex_count; i++)
        {
            normals[i] = -Vector3.forward;
        }

        m_Mesh.normals = normals;

        /*
		 * Mesh UVs
		 */
        m_Uv = new Vector2[vertex_count];

        for (int i = 0; i < m_Uv.Length; i++)
        {
            m_Uv[i] = new Vector2(0, 0);
        }

        m_Mesh.uv = m_Uv;

        Points.Clear();
    }



    /*
	 * Utility Methods
	 */

    private bool isCleanEar(int Ear)
    {
        /*
		 * Barycentric Technique is used to test
		 * if the reflective vertices are in selected ears
		 */

        float dot00;
        float dot01;
        float dot02;
        float dot11;
        float dot12;

        float invDenom;
        float U;
        float V;

        Vector2 v0 = Points[PolyPointList[Ear].PointPrev - 1] - Points[Ear - 1];
        Vector2 v1 = Points[PolyPointList[Ear].PointNext - 1] - Points[Ear - 1];
        Vector2 v2;

        int i = PolyPointList[0].ReflNext;

        while (i != -1)
        {
            v2 = Points[i - 1] - Points[Ear - 1];

            dot00 = Vector2.Dot(v0, v0);
            dot01 = Vector2.Dot(v0, v1);
            dot02 = Vector2.Dot(v0, v2);
            dot11 = Vector2.Dot(v1, v1);
            dot12 = Vector2.Dot(v1, v2);

            invDenom = 1 / (dot00 * dot11 - dot01 * dot01);
            U = (dot11 * dot02 - dot01 * dot12) * invDenom;
            V = (dot00 * dot12 - dot01 * dot02) * invDenom;

            if ((U > 0) && (V > 0) && (U + V < 1))
                return false;

            i = PolyPointList[i].ReflNext;
        }

        return true;
    }

    private bool isReflective(int P)
    {
        /*
		 * vector cross product is used to determin the reflectiveness of vertices
		 * because "Sin" values of angles are always - if the angle > 180 
		 */

        Vector2 v0 = Points[PolyPointList[P].PointPrev - 1] - Points[P - 1];
        Vector2 v1 = Points[PolyPointList[P].PointNext - 1] - Points[P - 1];

        Vector3 A = Vector3.Cross(v0, v1);

        if (A.z < 0)
            return true;

        return false;
    }

    private void RemoveEar(int Ear)
    {
        int PrevEar = PolyPointList[Ear].EarPrev;
        int NextEar = PolyPointList[Ear].EarNext;

        PolyPointList[Ear].isEar = false;

        if (PrevEar == -1)
        {
            PolyPointList[0].EarNext = NextEar;
        }
        else
        {
            PolyPointList[PrevEar].EarNext = NextEar;
        }

        if (NextEar != -1)
        {
            PolyPointList[NextEar].EarPrev = PrevEar;
        }
    }

    private void AddEar(int Ear)
    {
        int NextEar = PolyPointList[0].EarNext;

        PolyPointList[0].EarNext = Ear;

        PolyPointList[Ear].EarPrev = -1;
        PolyPointList[Ear].EarNext = NextEar;

        PolyPointList[Ear].isEar = true;

        if (NextEar != -1)
        {
            PolyPointList[NextEar].EarPrev = Ear;
        }
    }

    private void RemoverReflective(int P)
    {
        int PrevRefl = PolyPointList[P].ReflPrev;
        int NextRefl = PolyPointList[P].ReflNext;

        if (PrevRefl == -1)
        {
            PolyPointList[0].ReflNext = NextRefl;
        }
        else
        {
            PolyPointList[PrevRefl].ReflNext = NextRefl;
        }

        if (NextRefl != -1)
        {
            PolyPointList[NextRefl].ReflPrev = PrevRefl;
        }
    }

    private void AddReflective(int P)
    {
        int NextRefl = PolyPointList[0].ReflNext;

        PolyPointList[0].ReflNext = P;

        PolyPointList[P].ReflPrev = -1;
        PolyPointList[P].ReflNext = NextRefl;

        if (NextRefl != -1)
        {
            PolyPointList[NextRefl].ReflPrev = P;
        }
    }

    private void RemoveP(int P)
    {
        int NextP = PolyPointList[P].PointNext;
        int PrevP = PolyPointList[P].PointPrev;

        PolyPointList[PrevP].PointNext = NextP;
        PolyPointList[NextP].PointPrev = PrevP;

        if (PolyPointList[0].PointNext == P)
            PolyPointList[0].PointNext = NextP;

        --Pointcount;
    }
}