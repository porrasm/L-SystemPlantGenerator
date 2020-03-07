using System;
using System.Collections.Generic;
using UnityEngine;

public class LineMeshCreator2D {

    #region
    private Vector3 startPos;
    private float startWidth;
    private List<VectorLine2D> lines;

    private Stack<PointInfo> branching;
    #endregion

    public LineMeshCreator2D(Vector3 startPos, float startWidth) {
        this.startPos = startPos;
        this.startWidth = startWidth;
        branching = new Stack<PointInfo>();
        lines = new List<VectorLine2D>();
    }

    public void Branch() {
        branching.Push(LastPoint);
    }
    public void Debranch() {
        lastPoint = branching.Pop();
    }

    public void NextDirection(LineState state) {
        if (state.CurrentLength <= 0) {
            throw new Exception("Length was 0 or negative");
        }

        Vector3 nextPos = state.GetEndPos(LastPoint.Pos);

        NextPoint(nextPos, state, state.Width);
    }
    private void NextPoint(Vector3 nextPos, LineState state, float nextWidth = -1) {

        PointInfo p = LastPoint;

        if (nextWidth < 0) {
            nextWidth = p.Width;
        }

        if (Vector3.Distance(p.Pos, nextPos) <= 0) {
            throw new System.Exception("New line length was 0");
        }

        VectorLine2D newLine = VectorLine2D.New(p.Pos, nextPos, state, p.Width, nextWidth);
        lines.Add(newLine);
        p.Pos = newLine.EndPos;
        p.Width = newLine.EndWidth;
        lastPoint = p;
    }

    private PointInfo lastPoint;
    private PointInfo LastPoint {
        get {
            if (lines.Count == 0) {
                PointInfo p = new PointInfo();
                p.Pos = startPos;
                p.Width = startWidth;
                return p;
            }
            return lastPoint;
        }
    }

    public MeshContainer GenerateMesh() {

        MeshContainer cont = new MeshContainer();

        Bounds bounds = new Bounds();

        float xMin = 0, xMax = 0;
        float yMin = 0, yMax = 0;

        List<Vector3> vertices = new List<Vector3>();
        List<Color> colors = new List<Color>();
        List<int> triangles = new List<int>();

        for (int i = 0; i < lines.Count; i++) {

            int vIndex = vertices.Count;

            Vector3[] v = lines[i].GetVertices();

            foreach (Vector3 ver in v) {
                vertices.Add(ver);
                colors.Add(lines[i].State.Color.Color);

                if (ver.x > xMax) {
                    xMax = ver.x;
                }
                if (ver.x < xMin) {
                    xMin = ver.x;
                }
                if (ver.y > yMax) {
                    yMax = ver.y;
                }
                if (ver.y < yMin) {
                    yMin = ver.y;
                }
            }

            triangles.Add(vIndex);
            triangles.Add(vIndex + 1);
            triangles.Add(vIndex + 3);

            triangles.Add(vIndex + 3);
            triangles.Add(vIndex + 2);
            triangles.Add(vIndex);

            if (i > 0) {
                triangles.Add(vIndex);
                triangles.Add(vIndex - 2);
                triangles.Add(vIndex + 1);

                triangles.Add(vIndex);
                triangles.Add(vIndex - 1);
                triangles.Add(vIndex + 1);
            }
        }

        Vector3[] normals = new Vector3[vertices.Count];
        Vector2[] uv = new Vector2[vertices.Count];
        for (int i = 0; i < normals.Length; i++) {
            normals[i] = -Vector3.forward;
            uv[i] = new Vector2(0, 0);
        }

        if (vertices.Count > 65535) {
            Logger.Print("line count: " + lines.Count);
            Logger.Print("Too large mesh: " + vertices.Count);
            return default(MeshContainer);
        }

        Logger.Print("Vertex count: " + vertices.Count);

        Mesh mesh = new Mesh();

        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.colors = colors.ToArray();
        mesh.normals = normals;
        mesh.uv = uv;
        mesh.RecalculateBounds();
        mesh.RecalculateTangents();
        mesh.RecalculateNormals();

        cont.Mesh = mesh;
        cont.Bounds = BoundsFromMaxPoints(xMin, xMax, yMin, yMax);
        cont.ScaledBounds = cont.Bounds;

        return cont;
    }
    private Bounds BoundsFromMaxPoints(float xMin, float xMax, float yMin, float yMax) {

        float xExtent = (xMax - xMin) / 2;
        float yExtent = (yMax - yMin) / 2;

        float xCenter = xMin + xExtent;
        float yCenter = yMin + yExtent;

        Bounds bounds = new Bounds();
        bounds.center = new Vector3(xCenter, yCenter);
        bounds.extents = new Vector3(xExtent, yExtent);

        return bounds;
    }

    private struct PointInfo {
        public Vector3 Pos;
        public float Width;
    }
}