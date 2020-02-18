using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class PlantMeshGenerator : MonoBehaviour {

    #region fields
    [SerializeField]
    private float angle = 15, width = 0.05f, length = 0.25f, iterationFactor = 0.75f;

    [Space(20)]
    [SerializeField]
    private PlantSettings settings;
    public PlantSettings Settings { get => settings; }

    private LineMeshCreator2D creator;

    private LBranch tree;
    private LBranch node;
    #endregion

    private void OnValidate() {
        settings.Validate();
    }

    private void Start() {
        GeneratePlant();
    }
    private void Update() {
        if (Input.GetKeyDown(KeyCode.Space)) {
            GeneratePlant();
        }
    }

    public void GeneratePlant() {

        if (settings.UseSeed) {
            RNG.SetSeed(settings.Seed);
        } else {
            RNG.SetSeed(RNG.Integer);
        }

        long time = Timer.Time;

        string treeString = settings.Grammar.PerformIterations(settings.Axiom, settings.Iterations);
        Debug.Log("Tree string: " + treeString);
        AnalyzeRule(treeString);

        Debug.Log("Analyzed " + Timer.Passed(time));
        creator = new LineMeshCreator2D(Vector3.zero, width);
        BuildTreeMesh(tree);
        Debug.Log("Tree build " + Timer.Passed(time));
        Debug.Log("Tree count: " + tree.Count);
        GetComponent<MeshFilter>().mesh = creator.GenerateMesh();
        Debug.Log("Mesh built " + Timer.Passed(time));
    }

    private void AnalyzeRule(string treeString) {

        Debug.Log("Result: " + treeString.Length);

        tree = new LBranch();
        node = tree;

        foreach (char c in treeString) {
            AnalyzeCommand(c);
        }
    }
    private void AnalyzeCommand(char c) {
        if (char.ToLower(c) == 'f') {
            node = node.Append();
        }
        if (c == '+') {
            node.Orientation += angle;
        }
        if (c == '-') {
            node.Orientation -= angle;
        }
        if (c == '(') {
            node = node.AddChild();
        }
        if (c == ')') {
            node = node.Parent;
        }
    }

    public void BuildTreeMesh(LBranch node) {
        while (node != null) {

            if (!node.IsBranchRoot) {
                creator.NextDirection(node.GetOrientationDirection(), length);
            }

            foreach (LBranch child in node.Branches) {
                creator.Branch();
                BuildTreeMesh(child);
                creator.Debranch();
            }

            node = node.Next;
        }
    }

    private float LengthAtIteration(int iteration) {
        return Mathf.Pow(iterationFactor, iteration) * length;
    }
}

public class LBranch {
    public LBranch Parent { get; private set; }
    public LBranch Prev { get; private set; }
    public LBranch Next { get; private set; }
    public List<LBranch> Branches { get; private set; }
    public int Depth { get; private set; }

    public int Count { get; private set; }

    public float Orientation { get; set; }
    public float Width { get; set; }

    public bool IsBranchRoot { get => Prev == null; }
    public bool IsRoot { get => Prev == null && Parent == null; }
    public bool HasNext { get => Next != null; }

    public LBranch() {
        Branches = new List<LBranch>();
    }

    public LBranch AddChild() {
        LBranch child = new LBranch();
        child.Parent = this;
        child.Orientation = Orientation;
        child.Depth = Depth + 1;
        child.Width = Width;
        Branches.Add(child);
        return child;
    }
    public LBranch Append() {
        LBranch next = new LBranch();
        next.Parent = Parent;
        next.Prev = this;
        this.Next = next;
        next.Orientation = Orientation;
        next.Width = Width;
        return next;
    }

    public Vector3 GetOrientationDirection() {

        Vector3 dir = Vector3.up;

        return Quaternion.AngleAxis(Orientation, Vector3.forward) * dir;
    }
}