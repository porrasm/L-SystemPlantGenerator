using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class PlantMeshGenerator : MonoBehaviour {

    #region fields
    private LineMeshCreator2D creator;

    private LBranch tree;
    private LBranch node;

    [SerializeField]
    private PlantSettings settings = new PlantSettings();

    [SerializeField]
    private bool generateOnSettingChange;

    [SerializeField]
    private int generateTimeLimit = 1000;

    public PlantSettings Settings { get => settings; set => settings = value; }
    public bool GenerateOnSettingChange { get => generateOnSettingChange; set => generateOnSettingChange = value; }
    public int GenerateTimeLimit { get => generateTimeLimit; set => generateTimeLimit = value; }
    #endregion

    private void OnValidate() {
        Debug.Log("Validate");
        Settings.Validate();
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

        if (Settings.UseSeed) {
            RNG.SetSeed(Settings.Seed);
        } else {
            RNG.SetSeed(RNG.Integer);
        }

        long time = Timer.Time;

        string treeString = Settings.Grammar.PerformIterations(Settings.Axiom, Settings.Iterations);
        Debug.Log("Tree string: " + treeString);
        AnalyzeRule(treeString);

        Debug.Log("Analyzed " + Timer.Passed(time));
        creator = new LineMeshCreator2D(Vector3.zero, Settings.Properties.StartingLineWidth, Settings.Properties);
        BuildTreeMesh(tree);
        Debug.Log("Tree build " + Timer.Passed(time));
        Debug.Log("Tree count: " + tree.Count);
        GetComponent<MeshFilter>().mesh = creator.GenerateMesh();
        Debug.Log("Mesh built " + Timer.Passed(time));
    }

    private void AnalyzeRule(string treeString) {

        Debug.Log("Result: " + treeString.Length);

        tree = new LBranch(Settings.Properties.ToLineState());
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
            node.State.Orientation += Settings.Properties.DefaultAngle;
        }
        if (c == '-') {
            node.State.Orientation -= Settings.Properties.DefaultAngle;
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
                creator.NextDirection(node.GetOrientationDirection(Settings.Properties.AngleVariance.GetSeededFloat()), node.State.Length, node.State);
            }

            foreach (LBranch child in node.Branches) {
                creator.Branch();
                BuildTreeMesh(child);
                creator.Debranch();
            }

            node = node.Next;
        }
    }
}