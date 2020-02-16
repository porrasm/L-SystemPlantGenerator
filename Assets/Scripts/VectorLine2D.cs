using UnityEngine;

public struct VectorLine2D {
    public Vector3 StartPos;
    public Vector3 EndPos;
    public float StartWidth;
    public float EndWidth;

    public Vector3[] GetVertices() {
        Vector3[] vertices = new Vector3[4];

        Vector3 dir = EndPos - StartPos;
        Vector3 right = Vector3.Cross(dir, Vector3.forward);

        vertices[0] = StartPos + right * (StartWidth * 0.5f);
        vertices[1] = StartPos - right * (StartWidth * 0.5f);

        vertices[2] = EndPos + right * (EndWidth * 0.5f);
        vertices[3] = EndPos - right * (EndWidth * 0.5f);

        return vertices;
    }

    public static VectorLine2D New(Vector3 start, Vector3 end, float startW, float endW = -1) {
        if (startW < 0) {
            throw new System.Exception("Width cannot be negative");
        }
        if (endW < 0) {
            endW = startW;
        }

        VectorLine2D line = new VectorLine2D();
        line.StartPos = start;
        line.EndPos = end;
        line.StartWidth = startW;
        line.EndWidth = endW;

        return line;
    }
}