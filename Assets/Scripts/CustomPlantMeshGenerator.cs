using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomPlantMeshGenerator : IPlantMeshGenerator {

    #region fields
    public PlantMeshGenerator Generator { get; set; }
    private PlantMeshCreator creator;

    private MeshContainer plantMesh;
    #endregion

    public override void GeneratePlant() {
        RebuildMeshes(true);
    }

    public override void RebuildMeshes(bool autoResize = false) {
        creator = new PlantMeshCreator(Vector3.zero, Generator.Settings.Properties.StartingLineWidth);
        plantMesh = creator.BuildTreeMesh(tree);

        if (autoResize) {
            PrepareTransform();
            ResizePlant();
        }

        GetMesh(0).mesh = plantMesh.Mesh;
    }

    protected override void ResizePlant() {
        throw new System.NotImplementedException();
    }
}