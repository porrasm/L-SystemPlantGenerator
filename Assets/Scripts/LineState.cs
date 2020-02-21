using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class LineState {

    #region fields
    public float Orientation = 0;
    public float Width = 0.05f;
    public float Length = 0.25f;
    public Color Color = Color.white;
    #endregion

    public LineState Copy() {
        LineState copy = new LineState();
        copy.Orientation = Orientation;
        copy.Width = Width;
        copy.Length = Length;
        copy.Color = Color;
        return copy;
    }
}