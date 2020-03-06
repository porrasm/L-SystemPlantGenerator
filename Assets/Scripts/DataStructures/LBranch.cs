using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// replace with properties
[Serializable]
public class LBranch {
    public LBranch Root;
    public LBranch Parent;
    public LBranch Prev;
    public LBranch Next;
    public List<LBranch> Branches;
    public int Depth;

    [SerializeField]
    private int countIndex;

    public LineState State;

    public int Count { get => this.Root.countIndex; }
    public int Index {
        get {
            if (IsRoot) {
                return 0;
            } else {
                return countIndex;
            }
        }
    }
    public bool IsBranchRoot { get => Prev == null; }
    public bool IsRoot { get => Prev == null && Parent == null; }
    public bool HasNext { get => Next != null; }
    public bool HasChildren { get => Branches.Count > 0; }

    public LBranch(LineState state) {
        this.Root = this;
        Branches = new List<LBranch>();
        countIndex = 1;
        this.State = state;
    }

    public LBranch AddChild() {
        LBranch child = new LBranch(State.Copy());
        SetRoot(child);
        child.Parent = this;
        child.Depth = Depth + 1;
        Branches.Add(child);
        return child;
    }
    public LBranch Append() {
        LBranch next = new LBranch(State.Copy());
        SetRoot(next);
        next.Parent = Parent;
        next.Prev = this;
        this.Next = next;
        return next;
    }
    private void SetRoot(LBranch node) {
        node.Root = this.Root;
        node.countIndex = this.Root.Count;
        this.Root.countIndex++;
    }

    public void RemoveThis() {
        if (IsRoot) {
            throw new Exception("Cannot remove root");
        }

        Depth = -1;
        Parent.RemoveChildren();
    }
    public LBranch RemoveIndex(int index) {
        LBranch removed = RemoveIndexHelper(this, index);
        if (removed == null) {
            throw new Exception("Invalid index: " + index);
        }
        removed.Root = removed;
        removed.Parent = null;
        removed.Prev = null;
        return removed;
    }

    private static LBranch RemoveIndexHelper(LBranch node, int index) {
        if (node == null) {
            return null;
        }
        if (node.HasNext && node.Next.Index == index) {
            Logger.Print("Found index: " + index);
            LBranch n = node.Next;
            node.Next = null;
            return n;
        }
        for (int i = 0; i < node.Branches.Count; i++) {
            if (node.Branches[i].Index == index) {
                Logger.Print("Found child index: " + index);
                LBranch n = node.Branches[i];
                node.Branches.RemoveAt(i);
                return n;
            }
        }
        LBranch ret = RemoveIndexHelper(node.Next, index);
        if (ret != null) {
            return ret;
        }
        foreach (LBranch child in node.Branches) {
            ret = RemoveIndexHelper(child, index);
            if (ret != null) {
                return ret;
            }
        }
        return null;
    }

    private void RemoveChildren() {
        for (int i = 0; i < Branches.Count; i++) {
            if (Branches[i].Depth < 0) {
                Branches.RemoveAt(i);
                i--;
            }
        }
    }

    public Vector2 GetOrientationDirection() {
        Vector2 dir = Vector2.up;
        return Quaternion.AngleAxis(State.Orientation, Vector3.forward) * dir;
    }
    // optimize
    public Vector2 GetEndPos(Vector2 lastPos) {
        return lastPos + GetOrientationDirection().normalized * State.CurrentLength;
    }

    public override bool Equals(object obj) {
        if (GetType() != obj.GetType()) {
            return false;
        }
        LBranch other = (LBranch)obj;
        return Index == other.Index;
    }

    public override int GetHashCode() {
        return -2134847229 + Index.GetHashCode();
    }
}