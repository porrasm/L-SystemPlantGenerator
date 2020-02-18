using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(PlantMeshGenerator))]
public class PlantSettingsEditor : Editor {

    #region fields
    private string text = "";
    private int i;
    float a;

    [SerializeField]
    private string normalSerialization;
    #endregion

    public override void OnInspectorGUI() {
        base.OnInspectorGUI();

        GUILayout.Label("label text");

        GUILayout.BeginVertical();

        text = GUILayout.TextArea(text, 200);
        Debug.Log("Got text: " + text);

        i = InspectorGUI.IntegerField("new integer", i, 0, 100);
        a = InspectorGUI.FloatSlider("float", a, 0, 1);

        GUILayout.Space(20);

        if (GUILayout.Button("Randomize seed")) {
            Debug.Log("Random seed");
            Generator.Settings.Seed = RNG.Integer;
        }
        if (GUILayout.Button("Generate plant")) {
            Debug.Log("Generated plant");
            Generator.GeneratePlant();
        }
        GUILayout.EndVertical();
    }

    private PlantMeshGenerator Generator {
        get {
            return (PlantMeshGenerator)target;
        }
    }
}