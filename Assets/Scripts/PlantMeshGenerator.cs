using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

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

    [SerializeField]
    private MeshContainer plantMesh;

    public PlantSettings Settings { get => settings; set => settings = value; }
    public bool GenerateOnSettingChange { get => generateOnSettingChange; set => generateOnSettingChange = value; }
    public int GenerateTimeLimit { get => generateTimeLimit; set => generateTimeLimit = value; }
    public MeshContainer PlantMesh { get => plantMesh; }

    private string axiomTest = "f(+f)f(-f(+f)f(+f)f)f(-f)f(+f)f(-f(-f)f(-f)f)f(-f)f(+f)f(-f(-f)f(+f)f(-f(-f)f(-f)f)f(-f)f(-f)f(-f(-f)f(+f)f)f(-f)f(-f)f)f(-f)f(-f)f(-f(-f)f(-f)f)f(-f)f(+f)f(-f(+f)f(+f)f)f(-f)f(-f)f(+f(-f)f(-f(+f)f(+f)f)f(-f)f(+f)f(+f(-f)f(+f)f)f(-f)f(+f)f(-f(-f)f(+f)f(-f(+f)f(+f)f)f(+f)f(+f)f(+f(-f)f(+f)f)f(-f)f(-f)f)f(-f)f(-f)f(-f(-f)f(+f)f)f(-f)f(+f)f(-f(-f)f(+f)f)f(-f)f(-f)f(+f(-f)f(-f)f(+f(-f)f(+f)f)f(-f)f(+f)f(+f(-f)f(+f)f)f(-f)f(+f)f)f(-f)f(+f)f(-f(+f)f(+f)f)f(-f)f(+f(-f)f(-f)f)f(-f)f(+f)f)f(-f)f(+f)f(+f(-f)f)f(+f)f(+f)f(+f(+f)f(+f)f)f(-f)f(-f)f(-f(-f)f(-f)(f(-f(-f)f(+f)f)f(-f)f(+f)f(-f(-f)f)f(-f)f(+f)f)f(-f)f(-f)f(+f(-f)f(+f)f)f(+f)f(-f)f(+f(-f)f(-f)f)f(-f)f(+f(+f)f(+f)f(-f(-f)f(-f)f)f(-f)f(+f)f(+f(-f)f(-f)f)f(-f)f(+f)f)f(+f)f(-f(+f)f(+f)f)f(+f)f(+f)f(+f(+f)f(+f)f)f(-f)f(-f)f(+f(-f)f(-f)f(-f(-f)f(+f)f)f(+f)f(+f)f(-f(-f)f)f(-f)f(+f)f(-f(-f)f(-f)f(-f(+f)f(+f)f)f(+f)f(-f)f(+f(+f)f(+f)f)f(-f)f(-f)f)f(+f)f(+f)f(+f(-f)f(+f)f)f(-f)f(+f)f(+f(+f)f(+f)f)f(-f)f(-f(-f)f(-f)f(+f(-f)f(+f)f)f(+f)f(+f)f(+f(+f)f(+f)f)f(-f)f(-f)f)f(-f)f(+f)f(-f(-f)f(-f)f)f(-f)f(+f)f)f(+f)f(+f)f(-f(-f)f(-f)f)f(-f)(f(-f)f(+f(-f)f)f(-f)f(+f)f(-f(-f)f(-f)f(-f(-f)f(-f)f)f(-f)f(-f)f(-f(-f)f(+f)f)f(-f)f(-f)f)f(+f)f(+f)f(-f(-f)f(+f)f)f(+f))f(+f)f(-f(-f)f(-f)f)f(-f)f(-f(-f)f(+f)f(-f(-f)f(+f)f)f(+f)f(+f)f(+f(+f)f(+f)f)f(-f)f(-f)f)f(-f)f(+f)f(-f(-f)f(-f)f)f(+f)f(+f)f(-f(-f)f(+f)f)f(+f)f(+f)f)";
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

    private Transform meshes;
    private Transform Meshes {
        get {
            if (meshes == null) {
                meshes = transform.Find("Meshes");
            }
            return meshes;
        }
    }
    private MeshFilter GetMesh(int index) {
        return Meshes.GetChild(index).GetComponent<MeshFilter>();
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

        PrepareTransform();
        Debug.Log("Tree build " + Timer.Passed(time));
        Debug.Log("Tree count: " + tree.Count);
        plantMesh = creator.GenerateMesh();
        GetMesh(0).mesh = plantMesh.Mesh;
        Debug.Log("Mesh built " + Timer.Passed(time));

        ResizePlant();
    }

    private void PrepareTransform() {
        if (Meshes == null) {
            AddMeshObject();
        }
    }

    private void AddMeshObject() {
        GameObject meshes = new GameObject();
        meshes.name = "Meshes";
        meshes.transform.SetParent(transform);

        GameObject mesh = new GameObject();
        mesh.name = "Mesh 0";
        mesh.AddComponent<MeshFilter>();
        mesh.AddComponent<MeshRenderer>();
        mesh.transform.SetParent(meshes.transform);
    }

    private void ResizePlant() {

        if (!Settings.Properties.ScaleToWidth && !Settings.Properties.ScaleToLength) {
            Meshes.localScale = Vector3.one;
            return;
        }

        float width = PlantMesh.Bounds.extents.x * 2;
        float height = PlantMesh.Bounds.extents.y * 2;

        float multiplier = float.MaxValue;

        if (Settings.Properties.ScaleToWidth) {

            float wMult = Settings.Properties.TargetWidth.Value / width;
            if (wMult < multiplier) {
                multiplier = wMult;
            }
        }
        if (Settings.Properties.ScaleToLength) {
            Debug.Log("Settings.Properties.TargetLength.Value: " + Settings.Properties.TargetLength.Value);
            float hMult = Settings.Properties.TargetLength.Value / height;
            if (hMult < multiplier) {
                multiplier = hMult;
            }
        }

        Debug.Log("Bounds: " + plantMesh.Bounds);
        Debug.Log("Mult: " + multiplier);

        Meshes.localScale = Vector3.one * multiplier;
    }

    private void AnalyzeRule(string treeString) {

        Debug.Log("Result: " + treeString.Length);

        tree = new LBranch(Settings.Properties.ToLineState());
        node = tree;

        List<StringCommand> commands = CommandTools.GetCommands(treeString);

        foreach (StringCommand command in commands) {
            ExecuteCommand(command);
        }
    }

    private void ExecuteCommand(StringCommand s) {

        // Handle state parameters

        if (s.Type == StringCommand.CommandType.Command) {
            if (s.Command.Equals("f")) {
                node = node.Append();
            }
            if (s.Command.Equals("+")) {
                node.State.Orientation += node.State.Angle + Settings.Properties.AngleVariance.GetSeededFloat();
            }
            if (s.Command.Equals("-")) {
                node.State.Orientation -= node.State.Angle + Settings.Properties.AngleVariance.GetSeededFloat();
            }
            if (s.Command.Equals("(")) {
                node = node.AddChild();
            }
            if (s.Command.Equals(")")) {
                node = node.Parent;
            }
        } else if (s.Type == StringCommand.CommandType.CommandParameter) {
#if UNITY_EDITOR
            node.State.ExecuteParameterCommands(s);
#endif
        }
    }

    public void BuildTreeMesh(LBranch node) {
        while (node != null) {

            if (!node.IsBranchRoot) {
                creator.NextDirection(node.GetOrientationDirection(), node.State.Length, node.State);
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