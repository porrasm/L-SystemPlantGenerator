﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomPlantSlaveGenerator : IPlantMeshGenerator {
    #region fields
    private PlantMeshCreator creator;
    private LBranch node;
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
        tree = treeCreator.CreateTree(treeString);
    }
    public override void RebuildMeshes(bool autoResize = false) {
        creator = new PlantMeshCreator(Vector3.zero, Settings.Properties.StartingLineWidth);
        plantMesh = creator.BuildTreeMesh(tree);

        if (autoResize) {
            PrepareTransform();
            ResizePlant();
        }

        GetMesh(0).mesh = plantMesh.Mesh;
    }

    protected override void ResizePlant() {

        if (!Settings.Properties.ScaleToWidth && !Settings.Properties.ScaleToLength) {
            Meshes.localScale = Vector3.one;
            return;
        }

        float width = plantMesh.Bounds.extents.x * 2;
        float height = plantMesh.Bounds.extents.y * 2;

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

    public static CustomPlantSlaveGenerator Add(GameObject g, PlantMeshGenerator owner) {
        CustomPlantSlaveGenerator s = g.AddComponent<CustomPlantSlaveGenerator>();
        s.Owner = owner ?? throw new System.Exception("Owner was null");
        s.Initialize();
        return s;
    }
}