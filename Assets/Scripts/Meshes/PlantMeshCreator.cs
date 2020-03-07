using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlantMeshCreator {

    #region fields
    private LineMeshCreator2D meshCreator;
    #endregion

    public PlantMeshCreator(Vector3 startPos, float startWidth) {
        meshCreator = new LineMeshCreator2D(startPos, startWidth);
    }

    public MeshContainer BuildTreeMesh(Plant plant) {
        BuildLineMesh(plant, 0);
        return meshCreator.GenerateMesh();
    }
    
    private void BuildLineMesh(Plant plant, int root) {
        int partID = root;

        while (partID != -1) {

            Plant.PlantPart part = plant.GetPart(partID);

            if (!part.IsBranchRoot) {
                Plant.PlantPart prev = plant.GetPart(part.Prev);
                meshCreator.NextDirection(prev.State);
            }

            foreach (int child in part.Children) {
                meshCreator.Branch();
                BuildLineMesh(plant, child);
                meshCreator.Debranch();
            }

            partID = part.Next;
        }
    }
}