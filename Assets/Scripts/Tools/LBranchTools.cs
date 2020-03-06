using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class LBranchTools {

    #region fields
    private LBranch tree;
    private Dictionary<LBranch, Vector2> branchPositions;
    #endregion

    public int Count { get => branchPositions.Count; }
    public LBranch Tree { get => tree; }

    public LBranchTools(LBranch root) {
        if (!root.IsRoot) {
            throw new Exception("Tree was not root.");
        }
        tree = root;
        Initialize();
    }

    private void Initialize() {
        branchPositions = new Dictionary<LBranch, Vector2>();
        RecursivePositions(default, tree);
    }
    private void RecursivePositions(Vector2 pos, LBranch node) {
        if (node == null) {
            return;
        }
        Vector2 nextPos = node.IsBranchRoot ? pos : node.GetEndPos(pos);

        branchPositions.Add(node, nextPos);

        foreach (LBranch child in node.Branches) {
            RecursivePositions(nextPos, child);
        }
        RecursivePositions(nextPos, node.Next);
    }

    public LBranchInfo GetClosestPosition(Vector2 position) {

        float closestDistance = float.MaxValue;
        KeyValuePair<LBranch, Vector2> closest;

        foreach (KeyValuePair<LBranch, Vector2> pair in branchPositions) {
            float newDistance = Vector2.Distance(position, pair.Value);
            if (newDistance < closestDistance) {
                closestDistance = newDistance;
                closest = pair;
            }
        }

        if (closest.Key == null) {
            Logger.Print("Not found");
            return new LBranchInfo();
        }

        LBranchInfo info = new LBranchInfo();
        info.Node = closest.Key;
        info.Position = closest.Value;

        return info;
    }
}

public struct LBranchInfo {
    public LBranch Node;
    public Vector2 Position;
}
