using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class CustomPlantEditor {

    #region fields
    private PlantSettingsEditor editor;
    Rect rect = new Rect(10, 10, 50, 50);
    private bool pressed;

    private LBranchTools treeTools;

    public CustomPlantCreator CustomPlant { get; private set; }
    #endregion

    public CustomPlantEditor(PlantSettingsEditor editor) {
        this.editor = editor;
        CustomPlant = new CustomPlantCreator(editor.Generator);
    }

    private LBranchTools TreeTools {
        get {
            if (treeTools == null || !ReferenceEquals(treeTools.Tree, editor.Generator.Tree) || treeTools.Count != editor.Generator.Tree.Count) {
                treeTools = new LBranchTools(editor.Generator.Tree);
            }
            return treeTools;
        }
    }

    public void EditorGUI() {
        InspectorGUI.CreateBox();
        GUILayout.Label("Custom plant editor");
        if (GUILayout.Button("Exit editor")) {
            CustomPlant.Enabled = false;
        }
        if (GUILayout.Button("Finish")) {
        }
        InspectorGUI.EndArea();
    }

    public void Editor() {
        Handles.BeginGUI();

        Camera cam = SceneView.lastActiveSceneView.camera;

        Transform p = editor.Generator.transform;

        Vector3 center = p.position + new Vector3(editor.Generator.PlantMesh.ScaledBounds.extents.x, editor.Generator.PlantMesh.ScaledBounds.extents.y);
        center.z = 0;

        // incorrect
        center = HandleUtility.WorldToGUIPoint(center);

        rect.center = center;

        Rect plantArea = BoundsToRect(editor.Generator.PlantMesh.ScaledBounds, center);
        plantArea.size *= 1.1f;

        //GUI.Box(plantArea, "AREA");

        //if (GUI.Button(plantArea, "Drag")) {
        //    pressed = !pressed;
        //}

        Rect buttonArea = new Rect(0, 0, 15, 15);

        if (plantArea.Contains(Event.current.mousePosition) || true) {

            Vector3 pos;

            Vector3 mouseWorld = WorldPos(Event.current.mousePosition);
            Vector3 meshLocalMouse = editor.Generator.Meshes.InverseTransformPoint(mouseWorld);

            Vector3 localPos = editor.Generator.Meshes.InverseTransformPoint(p.position);

            LBranchInfo branchInfo = TreeTools.GetClosestPosition(meshLocalMouse);
            pos = editor.Generator.Meshes.TransformPoint(branchInfo.Position);

            buttonArea.position = GUIPos(pos) - new Vector2(buttonArea.width / 2, buttonArea.height / 2);
            if (GUI.Button(buttonArea, "")) {
                editor.Generator.Tree.RemoveIndex(branchInfo.Node.Index);
                editor.Generator.RebuildMeshes();
            }

            // prevents lag
            GameObject.FindGameObjectWithTag("Finish").transform.position = pos + Vector3.forward;
        } else {
            Logger.Print("not in area");
        }

        Handles.EndGUI();

    }

    private Vector2 GUIPos(Vector3 worldPos) {
        return HandleUtility.WorldToGUIPoint(worldPos);
    }
    private Vector3 WorldPos(Vector2 guiPos, Vector3 worldPos = default) {
        Vector3 pos = HandleUtility.GUIPointToWorldRay(guiPos).origin;
        pos.z = worldPos.z;
        return pos;
    }
    private Rect BoundsToRect(Bounds b, Vector2 center = default) {

        Vector2 lowerLeft = b.center - b.extents;
        Vector2 upperRight = b.center + b.extents;
        lowerLeft = GUIPos(lowerLeft);
        upperRight = GUIPos(upperRight);

        Vector2 size = upperRight - lowerLeft;
        size.y *= -1;

        Rect rect = new Rect();
        rect.size = size;
        rect.center = center;

        return rect;
    }
}