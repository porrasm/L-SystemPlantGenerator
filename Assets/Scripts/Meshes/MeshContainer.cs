using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct MeshContainer {
    public Mesh Mesh;
    public Bounds Bounds;
    public Bounds ScaledBounds;
    public float GetMultiplier() {
        return ScaledBounds.size.magnitude / Bounds.size.magnitude;
    }
    public void SetMultiplier(float multiplier) {
        ScaledBounds = Bounds;
        ScaledBounds.size = Bounds.size * multiplier;
    }
}