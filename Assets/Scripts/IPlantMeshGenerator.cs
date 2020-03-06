using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class IPlantMeshGenerator : MonoBehaviour {

    #region fields
    [SerializeField]
    protected LBranch tree;
    public LBranch Tree { get => tree; }

    protected Transform meshes;
    #endregion

    public Transform Meshes {
        get {
            if (meshes == null) {
                meshes = transform.Find("Meshes");
            }
            return meshes;
        }
    }
    protected MeshFilter GetMesh(int index) {
        return Meshes.GetChild(index).GetComponent<MeshFilter>();
    }

    protected void PrepareTransform() {
        if (Meshes == null) {
            AddMeshObject();
        }
        Meshes.localScale = Vector3.one;
        Meshes.localPosition = Vector3.zero;
        Meshes.localEulerAngles = Vector3.zero;
    }

    protected void AddMeshObject() {
        GameObject meshes = new GameObject();
        meshes.name = "Meshes";
        meshes.transform.SetParent(transform);

        GameObject mesh = new GameObject();
        mesh.name = "Mesh 0";
        mesh.AddComponent<MeshFilter>();
        mesh.AddComponent<MeshRenderer>();
        mesh.transform.SetParent(meshes.transform);
    }

    #region abstract methods
    public abstract void GeneratePlant();
    public abstract void RebuildMeshes(bool autoResize = false);
    protected abstract void ResizePlant();
    #endregion
}