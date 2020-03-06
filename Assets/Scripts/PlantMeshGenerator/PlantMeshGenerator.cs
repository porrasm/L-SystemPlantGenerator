using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class PlantMeshGenerator : IPlantMeshGenerator {

    #region fields
    private PlantMeshCreator creator;

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

    private TreeCreator treeCreator;

    private string axiomTest = "f(+f)f(-f(+f)f(+f)f)f(-f)f(+f)f(-f(-f)f(-f)f)f(-f)f(+f)f(-f(-f)f(+f)f(-f(-f)f(-f)f)f(-f)f(-f)f(-f(-f)f(+f)f)f(-f)f(-f)f)f(-f)f(-f)f(-f(-f)f(-f)f)f(-f)f(+f)f(-f(+f)f(+f)f)f(-f)f(-f)f(+f(-f)f(-f(+f)f(+f)f)f(-f)f(+f)f(+f(-f)f(+f)f)f(-f)f(+f)f(-f(-f)f(+f)f(-f(+f)f(+f)f)f(+f)f(+f)f(+f(-f)f(+f)f)f(-f)f(-f)f)f(-f)f(-f)f(-f(-f)f(+f)f)f(-f)f(+f)f(-f(-f)f(+f)f)f(-f)f(-f)f(+f(-f)f(-f)f(+f(-f)f(+f)f)f(-f)f(+f)f(+f(-f)f(+f)f)f(-f)f(+f)f)f(-f)f(+f)f(-f(+f)f(+f)f)f(-f)f(+f(-f)f(-f)f)f(-f)f(+f)f)f(-f)f(+f)f(+f(-f)f)f(+f)f(+f)f(+f(+f)f(+f)f)f(-f)f(-f)f(-f(-f)f(-f)(f(-f(-f)f(+f)f)f(-f)f(+f)f(-f(-f)f)f(-f)f(+f)f)f(-f)f(-f)f(+f(-f)f(+f)f)f(+f)f(-f)f(+f(-f)f(-f)f)f(-f)f(+f(+f)f(+f)f(-f(-f)f(-f)f)f(-f)f(+f)f(+f(-f)f(-f)f)f(-f)f(+f)f)f(+f)f(-f(+f)f(+f)f)f(+f)f(+f)f(+f(+f)f(+f)f)f(-f)f(-f)f(+f(-f)f(-f)f(-f(-f)f(+f)f)f(+f)f(+f)f(-f(-f)f)f(-f)f(+f)f(-f(-f)f(-f)f(-f(+f)f(+f)f)f(+f)f(-f)f(+f(+f)f(+f)f)f(-f)f(-f)f)f(+f)f(+f)f(+f(-f)f(+f)f)f(-f)f(+f)f(+f(+f)f(+f)f)f(-f)f(-f(-f)f(-f)f(+f(-f)f(+f)f)f(+f)f(+f)f(+f(+f)f(+f)f)f(-f)f(-f)f)f(-f)f(+f)f(-f(-f)f(-f)f)f(-f)f(+f)f)f(+f)f(+f)f(-f(-f)f(-f)f)f(-f)(f(-f)f(+f(-f)f)f(-f)f(+f)f(-f(-f)f(-f)f(-f(-f)f(-f)f)f(-f)f(-f)f(-f(-f)f(+f)f)f(-f)f(-f)f)f(+f)f(+f)f(-f(-f)f(+f)f)f(+f))f(+f)f(-f(-f)f(-f)f)f(-f)f(-f(-f)f(+f)f(-f(-f)f(+f)f)f(+f)f(+f)f(+f(+f)f(+f)f)f(-f)f(-f)f)f(-f)f(+f)f(-f(-f)f(-f)f)f(+f)f(+f)f(-f(-f)f(+f)f)f(+f)f(+f)f)";
    #endregion

    public void Initialize() {
        treeCreator = new TreeCreator(Settings);
    }

    private void OnValidate() {
        Logger.Print("Validate");
        Initialize();
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

    public override void GeneratePlant() {
        RegenerateTree();
        RebuildMeshes(true);
    }
    public void RegenerateTree() {
        if (Settings.UseSeed) {
            RNG.SetSeed(Settings.Seed);
        } else {
            RNG.SetSeed(RNG.Integer);
        }

        string treeString = Settings.Grammar.PerformIterations(Settings.Axiom, Settings.Iterations);
        Logger.Print("Tree string: " + treeString);
        tree = treeCreator.CreateTree(treeString);
    }
    public override void RebuildMeshes(bool autoResize = false) {
        creator = new PlantMeshCreator(Vector3.zero, Settings.Properties.StartingLineWidth);
        plantMesh = creator.BuildTreeMesh(tree);

        if (autoResize) {
            PrepareTransform();
            ResizePlant();
        }

        Meshes.gameObject.SetActive(true);
        CleanTransform();

        GetMesh(0).mesh = plantMesh.Mesh;
    }

    protected override void ResizePlant() {

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
            Logger.Print("Settings.Properties.TargetLength.Value: " + Settings.Properties.TargetLength.Value);
            float hMult = Settings.Properties.TargetLength.Value / height;
            if (hMult < multiplier) {
                multiplier = hMult;
            }
        }

        Logger.Print("Bounds: " + plantMesh.Bounds);
        Logger.Print("Mult: " + multiplier);

        Meshes.localScale = Vector3.one * multiplier;

        plantMesh.ScaledBounds = new Bounds(plantMesh.Bounds.center, plantMesh.Bounds.size * multiplier);
    }

    public void CleanTransform() {
        foreach (Transform child in transform) {
            if (!child.name.Equals("Meshes")) {
                DestroyImmediate(child.gameObject);
            }
        }
    }
}