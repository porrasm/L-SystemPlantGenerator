using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LBranch {
    public LBranch Parent { get; private set; }
    public LBranch Prev { get; private set; }
    public LBranch Next { get; private set; }
    public List<LBranch> Branches { get; private set; }
    public int Depth { get; private set; }

    public int Count { get; private set; }

    public LineState State { get; set; }

    public bool IsBranchRoot { get => Prev == null; }
    public bool IsRoot { get => Prev == null && Parent == null; }
    public bool HasNext { get => Next != null; }

    public LBranch(LineState state) {
        Branches = new List<LBranch>();
        this.State = state;
    }

    public LBranch AddChild() {
        LBranch child = new LBranch(State.Copy());
        child.Parent = this;
        child.Depth = Depth + 1;
        Branches.Add(child);
        return child;
    }
    public LBranch Append() {
        LBranch next = new LBranch(State.Copy());
        next.Parent = Parent;
        next.Prev = this;
        this.Next = next;
        return next;
    }

    public Vector3 GetOrientationDirection() {

        Vector3 dir = Vector3.up;

        return Quaternion.AngleAxis(State.Orientation, Vector3.forward) * dir;
    }
}