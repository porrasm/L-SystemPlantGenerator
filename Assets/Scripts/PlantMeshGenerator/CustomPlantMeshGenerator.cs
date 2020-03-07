using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomPlantMeshGenerator : IPlantMeshGenerator {

    #region fields
    private PlantMeshGenerator owner;
    private PlantMeshCreator creator;

    private PlantSettings Settings { get => owner.Settings; }
    #endregion

    public override void GeneratePlant() {
        RebuildMeshes(true);
    }

    public override void RebuildMeshes(bool autoResize = false) {
        if (plant == null) {
            GetMesh(0).mesh = null;
            return;
        }
        creator = new PlantMeshCreator(Vector3.zero, plant.GetPart(0).State.Width);
        plantMesh = creator.BuildTreeMesh(plant);

        if (autoResize) {
            PrepareTransform();
            ResizePlant();
        }

        GetMesh(0).mesh = plantMesh.Mesh;
    }

    protected override void ResizePlant() {
        Meshes.transform.localScale = owner.Meshes.localScale;
        PlantMesh.SetMultiplier(owner.PlantMesh.GetMultiplier());
    }

    public static CustomPlantMeshGenerator Add(GameObject g, PlantMeshGenerator owner) {
        CustomPlantMeshGenerator p = g.AddComponent<CustomPlantMeshGenerator>();
        p.owner = owner;
        return p;
    }
}