using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class LBranchTools {

    #region fields
    private Plant plant;
    private Dictionary<int, Vector2> branchPositions;
    #endregion

    public int Count { get => branchPositions.Count; }
    public Plant Plant { get => plant; }

    public LBranchTools(Plant plant) {
        this.plant = plant;
        Initialize();
    }

    private void Initialize() {
        branchPositions = new Dictionary<int, Vector2>();
        RecursivePositions(default, 0);
    }
    private void RecursivePositions(Vector2 pos, int partIndex) {
        while (partIndex != -1) {
            Plant.PlantPart part = plant.GetPart(partIndex);
            Vector2 nextPos = part.IsBranchRoot ? pos : part.State.GetEndPos(pos);

            branchPositions.Add(partIndex, nextPos);

            foreach (int child in part.Children) {
                RecursivePositions(nextPos, child);
            }
            pos = nextPos;
            partIndex = part.Next;
        }
    }

    public LBranchInfo GetClosestPosition(Vector2 position) {

        float closestDistance = float.MaxValue;
        KeyValuePair<int, Vector2> closest = new KeyValuePair<int, Vector2>(-1, new Vector2());

        foreach (KeyValuePair<int, Vector2> pair in branchPositions) {
            float newDistance = Vector2.Distance(position, pair.Value);
            if (newDistance < closestDistance) {
                closestDistance = newDistance;
                closest = pair;
            }
        }

        if (closest.Key == -1) {
            Logger.Print("Not found");
            return new LBranchInfo();
        }

        LBranchInfo info = new LBranchInfo();
        info.Plant = plant;
        info.PartID = closest.Key;
        info.Position = closest.Value;

        return info;
    }
}

public struct LBranchInfo {
    public Plant Plant;
    public int PartID;
    public Plant.PlantPart Part { get { return Plant.GetPart(PartID); } }
    public Vector2 Position;
}
