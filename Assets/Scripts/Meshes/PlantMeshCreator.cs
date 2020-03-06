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

    public MeshContainer BuildTreeMesh(LBranch tree) {
        BuildLineMesh(tree);
        return meshCreator.GenerateMesh();
    }

    private void BuildLineMesh(LBranch node) {
        while (node != null) {

            if (!node.IsBranchRoot) {
                meshCreator.NextDirection(node.Prev.GetOrientationDirection(), node.State);
            }

            foreach (LBranch child in node.Branches) {
                meshCreator.Branch();
                BuildLineMesh(child);
                meshCreator.Debranch();
            }

            node = node.Next;
        }
    }
}