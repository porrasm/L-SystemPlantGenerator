using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomPlantSlaveGenerator : IPlantMeshGenerator {
    #region fields
    private PlantMeshCreator creator;
    private PlantMeshGenerator Owner { get; set; }
    private TreeCreator treeCreator;
    public PlantSettings Settings { get => Owner?.Settings; }
    #endregion

    private void Initialize() {
        treeCreator = new TreeCreator(Settings);
    }

    private void OnValidate() {
        Logger.Print("Validate");
        Settings?.Validate();
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
        plant = treeCreator.CreatePlant(treeString);
    }
    public override void RebuildMeshes(bool autoResize = false) {
        creator = new PlantMeshCreator(Vector3.zero, Settings.Properties.StartingLineWidth);
        plantMesh = creator.BuildTreeMesh(plant);

        if (autoResize) {
            PrepareTransform();
            ResizePlant();
        }

        GetMesh(0).mesh = plantMesh.Mesh;
    }

    protected override void ResizePlant() {
        Meshes.localScale = Owner.Meshes.localScale;
        PlantMesh.SetMultiplier(Owner.PlantMesh.GetMultiplier());
    }

    public static CustomPlantSlaveGenerator Add(GameObject g, PlantMeshGenerator owner) {
        CustomPlantSlaveGenerator s = g.AddComponent<CustomPlantSlaveGenerator>();
        s.Owner = owner ?? throw new System.Exception("Owner was null");
        s.Initialize();
        return s;
    }
}