using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

[Serializable]
public class Plant {

    #region fields
    [SerializeField]
    private List<int> keys;
    [SerializeField]
    private List<PlantPart> values;
    [NonSerialized]
    private Dictionary<int, PlantPart> parts;
    [NonSerialized]
    private int index;
    #endregion

    public int Count { get => parts.Count; }

    public void OnBeforeSerialize() {
        keys = new List<int>();
        values = new List<PlantPart>();

        foreach (var pair in parts) {
            keys.Add(pair.Key);
            values.Add(pair.Value);
        }
    }

    public void OnAfterDeserialize() {
        parts = new Dictionary<int, PlantPart>();

        for (int i = 0; i < keys.Count; i++) {
            parts.Add(keys[i], values[i]);
        }
        index = parts.Count;
    }

    public Plant(LineState initialState) {
        parts = new Dictionary<int, PlantPart>();
        AddPart(initialState, index++);
    }

    #region modification
    public int AppendAfter(int prevIndex) {
        PlantPart prev = parts[prevIndex];
        PlantPart added = AddPart(prev.State, index++, prev.Parent, prevIndex);
        prev.Next = added.ID;
        return added.ID;
    }
    public int AppenUnder(int prevIndex) {
        PlantPart prev = parts[prevIndex];
        PlantPart added = AddPart(prev.State, index++, prevIndex);
        prev.Children.Add(added.ID);
        return added.ID;
    }
    private PlantPart AddPart(LineState state, int index, int parent = -1, int prev = -1) {
        PlantPart part = PlantPart.New(state, index, parent, prev);
        parts.Add(part.ID, part);
        return part;
    }

    public Plant Remove(int part) {
        throw new NotImplementedException();
    }

    public void Merge(Plant plant, int parent) {
        throw new NotImplementedException();
    }

    public Plant Cut(int id) {
        throw new NotImplementedException();
    }
    #endregion

    public PlantPart GetPart(int id) {
        return parts[id];
    }

    public struct PlantPart {
        public int Prev;
        public int Next;
        public int Parent;
        public int ID;
        public LineState State;
        public List<int> Children;

        public static PlantPart New(LineState state, int id, int parent, int prev) {
            PlantPart part = new PlantPart();
            part.Parent = parent;
            part.Prev = prev;
            part.ID = id;
            part.Children = new List<int>();
            part.State = state.Copy();
            return part;
        }

        public bool IsRoot {
            get {
                return Parent == -1 && Prev == -1;
            }
        }
        public bool IsBranchRoot {
            get {
                return Prev == -1;
            }
        }
    }
}
