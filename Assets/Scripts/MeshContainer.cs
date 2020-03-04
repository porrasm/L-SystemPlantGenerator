using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct MeshContainer {
    public Mesh Mesh;
    public Bounds Bounds;
    public Bounds ScaledBounds;
}