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
    public LBranch AddChild(LBranch child) {
        LBranch added = AddChild();
        added.State = child.State.Copy();
        return added;
    }
    public LBranch Append() {
        LBranch next = new LBranch(State.Copy());
        SetRoot(next);
        next.Parent = Parent;
        next.Prev = this;
        this.Next = next;
        return next;
    }
    public LBranch Append(LBranch node) {
        LBranch appended = Append();
        appended.State = node.State.Copy();
        return appended;
    }

    private void SetRoot(LBranch node) {
        node.Root = this.Root;
        node.countIndex = this.Root.Count;
        this.Root.countIndex++;
    }

    public void Remove(LBranch node) {

        if (Root != node.Root) {
            throw new Exception("Node is not in tree");
        }

        LBranch prev = node.Prev;
        if (prev != null) {
            prev.Next = null;
        }
        LBranch parent = node.Parent;
        if (parent != null) {
            parent.RemoveChild(node.Index);
        }

        node.Root = node;
        node.Parent = null;
        node.Prev = null;

        FixIndexes();
    }

    private void RemoveChild(int index) {
        for (int i = 0; i < Branches.Count; i++) {
            if (Branches[i].Index == index) {
                Branches.RemoveAt(i);
                return;
            }
        }
    }
    private void FixIndexes() {
        Root.countIndex = FixIndexesHelper(Root, 0, 0);
    }
    private int FixIndexesHelper(LBranch node, int index, int depth) {
        node.countIndex = index;
        node.Depth = depth;

        int count = 1;
        foreach (LBranch child in node.Branches) {
            count += FixIndexesHelper(child, index + 1, depth + 1);
        }
        if (node.HasNext) {
            count += FixIndexesHelper(node.Next, index + 1, depth);
        }

        return count;
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

    private List<LBranch> GetAllNodes() {
        Queue<LBranch> nodes = new Queue<LBranch>();
        nodes.Enqueue(this);

        List<LBranch> ordered = new List<LBranch>();

        while (nodes.Count > 0) {
            LBranch node = nodes.Dequeue();
            ordered.Add(node);

            foreach (LBranch child in node.Branches) {
                nodes.Enqueue(child);
            }
            if (node.HasNext) {
                nodes.Enqueue(node.Next);
            }
        }

        return ordered;
    }

    public override int GetHashCode() {
        return -2134847229 + Index.GetHashCode();
    }

    public static void Merge(LBranch addToStart, LBranch toAddStart) {
        MergeHelper(addToStart, toAddStart);
    }
    private static void MergeHelper(LBranch addTo, LBranch toAdd) {
        while (toAdd.HasNext) {
            foreach (LBranch child in toAdd.Branches) {
                LBranch newAddTo = addTo.AddChild(child);
                MergeHelper(newAddTo, child);
            }
            if (toAdd.HasNext) {
                LBranch newAddTo = addTo.Append(toAdd);
                toAdd = toAdd.Next;
            }
        }
    }
}