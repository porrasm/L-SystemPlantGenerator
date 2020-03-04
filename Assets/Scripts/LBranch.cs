using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// replace with properties
[Serializable]
public class LBranch {
    public LBranch Parent;
    public LBranch Prev;
    public LBranch Next;
    public List<LBranch> Branches;
    public int Depth;

    public int Count;

    public LineState State;

    public bool IsBranchRoot { get => Prev == null; }
    public bool IsRoot { get => Prev == null && Parent == null; }
    public bool HasNext { get => Next != null; }
    public bool HasChildren { get => Branches.Count > 0; }

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
    public void Remove() {
        if (IsRoot) {
            throw new Exception("Cannot remove root");
        }

        Depth = -1;
        Parent.RemoveChildren();
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

    #region helpers
    public LBranch GetClosestChildNode(Vector2 startPos, Vector2 targetPos, out Vector3 nodePosition) {

        Vector2 currentPos = startPos;

        Stack<LBranch> nodes = new Stack<LBranch>();

        float closestDistance = float.MaxValue;
        Vector3 closestPosition = default;
        LBranch closest = null;

        nodes.Push(this);

        Debug.Log("------------------------------------");
        Debug.Log("Mouse pos: " + targetPos);

        while (nodes.Count > 0) {
            LBranch current = nodes.Pop();
            currentPos = current.GetEndPos(currentPos);

            float distance = Vector2.Distance(currentPos, targetPos);

            Debug.Log("Current pos: " + currentPos + ", target: " + targetPos);
            Debug.Log("Current distance: " + distance);

            if (current.Depth > 0) {
                Debug.Log("Handle child: " + currentPos);
            }

            if (distance < closestDistance) {
                Debug.Log("Found new closest: " + currentPos);
                Debug.Log("Distance: " + closestDistance);
                closestDistance = distance;
                closest = current;
                closestPosition = currentPos;
            }

            foreach (LBranch b in current.Branches) {
                nodes.Push(b);
            }
            if (current.Next != null) {
                nodes.Push(current.Next);
            }
        }

        if (closest.IsRoot) {
            Debug.Log("Returning root");
            nodePosition = default;
            return null;
        }

        Debug.Log("Returning closest, dsitance_ " + closestDistance + "; pos " + closestPosition);
        nodePosition = closestPosition;
        return closest;
    }
    #endregion
}