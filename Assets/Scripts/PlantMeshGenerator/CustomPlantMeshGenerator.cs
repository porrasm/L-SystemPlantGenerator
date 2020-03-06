using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomPlantMeshGenerator : IPlantMeshGenerator {

    #region fields
    private PlantSettings settings;
    private PlantMeshCreator creator;
    #endregion

    public override void GeneratePlant() {
        RebuildMeshes(true);
    }

    public override void RebuildMeshes(bool autoResize = false) {
        creator = new PlantMeshCreator(Vector3.zero, tree.State.Width);
        plantMesh = creator.BuildTreeMesh(tree);

        if (autoResize) {
            PrepareTransform();
            ResizePlant();
        }

        GetMesh(0).mesh = plantMesh.Mesh;
    }

    protected override void ResizePlant() {
        if (!settings.Properties.ScaleToWidth && !settings.Properties.ScaleToLength) {
            Meshes.localScale = Vector3.one;
            return;
        }

        float width = plantMesh.Bounds.extents.x * 2;
        float height = plantMesh.Bounds.extents.y * 2;

        float multiplier = float.MaxValue;

        if (settings.Properties.ScaleToWidth) {

            float wMult = settings.Properties.TargetWidth.Value / width;
            if (wMult < multiplier) {
                multiplier = wMult;
            }
        }
        if (settings.Properties.ScaleToLength) {
            Logger.Print("Settings.Properties.TargetLength.Value: " + settings.Properties.TargetLength.Value);
            float hMult = settings.Properties.TargetLength.Value / height;
            if (hMult < multiplier) {
                multiplier = hMult;
            }
        }

        Logger.Print("Bounds: " + plantMesh.Bounds);
        Logger.Print("Mult: " + multiplier);

        Meshes.localScale = Vector3.one * multiplier;

        plantMesh.ScaledBounds = new Bounds(plantMesh.Bounds.center, plantMesh.Bounds.size * multiplier);
    }

    public static CustomPlantMeshGenerator Add(GameObject g, PlantSettings settings) {
        CustomPlantMeshGenerator p = g.AddComponent<CustomPlantMeshGenerator>();
        p.settings = settings;
        return p;
    } 
}