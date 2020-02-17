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

    private void NextPoint(Vector3 nextPos, float nextWidth = -1) {

        PointInfo p = LastPoint;

        if (nextWidth < 0) {
            nextWidth = p.Width;
        }

        if (Vector3.Distance(p.Pos, nextPos) <= 0) {
            throw new System.Exception("New line length was 0");
        }

        VectorLine2D newLine = VectorLine2D.New(p.Pos, nextPos, p.Width, nextWidth);
        lines.Add(newLine);
        p.Pos = nextPos;
        p.Width = nextWidth;
        lastPoint = p;
    }

    public void NextDirection(Vector3 direction, float length, float nextWidth = -1) {
        if (length <= 0) {
            throw new Exception("Length was 0 or negative");
        }
        if (direction.normalized == Vector3.zero) {
            throw new Exception("Invalid direction");
        }

        Vector3 nextPos = LastPoint.Pos + direction.normalized * length;

        NextPoint(nextPos, nextWidth);
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

    


    public Mesh GenerateMesh() {

        List<Vector3> vertices = new List<Vector3>();
        List<int> triangles = new List<int>();

        for (int i = 0; i < lines.Count; i++) {

            int vIndex = vertices.Count;

            Vector3[] v = lines[i].GetVertices();

            foreach (Vector3 ver in v) {
                vertices.Add(ver);
            }

            triangles.Add(vIndex);
            triangles.Add(vIndex + 1);
            triangles.Add(vIndex + 3);

            triangles.Add(vIndex);
            triangles.Add(vIndex + 2);
            triangles.Add(vIndex + 3);

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
            Debug.Log("line count: " + lines.Count);
            Debug.Log("Too large mesh: " + vertices.Count);
            return null;
        }

        Debug.Log("Vertex count: " + vertices.Count);

        Mesh mesh = new Mesh();

        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        // mesh.normals = normals;
        // mesh.uv = uv;
        mesh.RecalculateBounds();
        mesh.RecalculateTangents();
        mesh.RecalculateNormals();

        return mesh;
    }

    private struct PointInfo {
        public Vector3 Pos;
        public float Width;
    }
}