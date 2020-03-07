using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class CustomPlantEditor {

    #region fields
    private PlantSettingsEditor editor;
    Rect rect = new Rect(10, 10, 50, 50);
    private bool pressed;

    public CustomPlantCreator CustomPlant { get; private set; }

    private Vector2 mouseWorldPos;

    private bool customSelected;

    private delegate void GUIButtonCallback(LBranchInfo info);

    private LBranchTools[] plantTools;
    #endregion

    public CustomPlantEditor(PlantSettingsEditor editor) {
        this.editor = editor;
        CustomPlant = new CustomPlantCreator(editor.Generator);
        plantTools = new LBranchTools[2];
    }

    private LBranchTools CustomPlantTools {
        get {
            if (plantTools[0] == null) {
                RecalculateTools();
            }
            return plantTools[0];
        }
    }
    private LBranchTools CustomPartsTools {
        get {
            if (plantTools[1] == null) {
                RecalculateTools();
            }
            return plantTools[0];
        }
    }
    private void RecalculateTools() {
        plantTools[0] = new LBranchTools(CustomPlant.customPlantGenerator.Plant);
        plantTools[1] = new LBranchTools(CustomPlant.customPlantPartsGenerator.Plant);
    }

    public void Initialize() {
        CustomPlant.Initialize();
        RecalculateTools();
    }

    public void EditorGUI() {
        InspectorGUI.CreateBox();
        GUILayout.Label("Custom plant editor");
        if (GUILayout.Button("Exit editor")) {
            editor.DisableCustom();
        }
        if (GUILayout.Button("Finish")) {
        }
        InspectorGUI.EndArea();
    }

    public void Editor() {

        mouseWorldPos = ToWorldPos(Event.current.mousePosition);

        float customDistance = Vector2.Distance(CustomPlant.customPlantGenerator.transform.position, mouseWorldPos);
        float partsDistance = Vector2.Distance(CustomPlant.customPlantPartsGenerator.transform.position, mouseWorldPos);

        Handles.BeginGUI();
        if (customSelected) {
            CustomSelectedFunctions();
        } else if (customDistance < partsDistance) {
            CustomPlantFunctions();
        } else {
            CustomPartsFunctions();
        }
        Handles.EndGUI();

        CustomPlant.Update();
    }

    private void CustomSelectedFunctions() {
        Rect buttonArea = new Rect(0, 0, 15, 15);
        buttonArea.position = ToGuiPos(mouseWorldPos) - new Vector2(buttonArea.width / 2, buttonArea.height / 2);
        
        void CB(LBranchInfo closest) {
            Logger.Print("Merging tree");
            closest.Plant.Merge(CustomPlant.selectedPartGenerator.Plant, closest.PartID);

            Deselect();
            CustomPlant.customPlantGenerator.GeneratePlant();
            RecalculateTools();
        }
        Rect acceptArea = GUIButton(CB, CustomPlant.customPlantGenerator.Meshes.transform, CustomPlantTools, "✓");

        if (!acceptArea.Contains(buttonArea.center)) {
            if (GUI.Button(buttonArea, "X")) {
                Logger.Print("Disable grab");
                Deselect();
            }
        }
    }
    private void CustomPlantFunctions() {
        void CB(LBranchInfo branchInfo) {
            branchInfo.Plant.Remove(branchInfo.PartID);

            Select(branchInfo.Plant, branchInfo.PartID);
            CustomPlant.customPlantGenerator.GeneratePlant();
            RecalculateTools();
        }

        GUIButton(CB, CustomPlant.customPlantGenerator.Meshes.transform, CustomPlantTools);
    }
    private void CustomPartsFunctions() {
        void CB(LBranchInfo branchInfo) {
            Select(branchInfo.Plant, branchInfo.PartID);
        }

        GUIButton(CB, CustomPlant.customPlantPartsGenerator.Meshes.transform, CustomPartsTools);
    }
    private void Select(Plant plant, int part) {
        CustomPlant.selectedPartGenerator.Plant = plant.Cut(part);
        CustomPlant.selectedPartGenerator.GeneratePlant();
        customSelected = true;
    }
    private void Deselect() {
        CustomPlant.selectedPartGenerator.Plant = null;
        CustomPlant.selectedPartGenerator.GeneratePlant();
        customSelected = false;
    }

    private Rect GUIButton(GUIButtonCallback cb, Transform t, LBranchTools tools, string text = "") {
        Rect buttonArea = new Rect(0, 0, 15, 15);

        Vector2 localMouse = t.InverseTransformPoint(mouseWorldPos);
        LBranchInfo branchInfo = tools.GetClosestPosition(localMouse);
        Vector2 nodePos = t.TransformPoint(branchInfo.Position);

        buttonArea.position = ToGuiPos(nodePos) - new Vector2(buttonArea.width / 2, buttonArea.height / 2);
        if (GUI.Button(buttonArea, text)) {
            cb?.Invoke(branchInfo);
        }
        return buttonArea;
    }


    public static Vector2 ToGuiPos(Vector3 worldPos) {
        return HandleUtility.WorldToGUIPoint(worldPos);
    }
    public static Vector3 ToWorldPos(Vector2 guiPos, Vector3 worldPos = default) {
        Vector3 pos = HandleUtility.GUIPointToWorldRay(guiPos).origin;
        pos.z = worldPos.z;
        return pos;
    }
    public static Rect BoundsToRect(Bounds b, Vector2 center = default) {

        Vector2 lowerLeft = b.center - b.extents;
        Vector2 upperRight = b.center + b.extents;
        lowerLeft = ToGuiPos(lowerLeft);
        upperRight = ToGuiPos(upperRight);

        Vector2 size = upperRight - lowerLeft;
        size.y *= -1;

        Rect rect = new Rect();
        rect.size = size;
        rect.center = center;

        return rect;
    }
}