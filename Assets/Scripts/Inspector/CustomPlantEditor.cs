using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class CustomPlantEditor {

    #region fields
    private PlantSettingsEditor editor;
    Rect rect = new Rect(10, 10, 50, 50);
    private bool pressed;
    #endregion

    public CustomPlantEditor(PlantSettingsEditor editor) {
        this.editor = editor;
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

        Handles.color = Color.yellow;

        Handles.Label(p.position, "ASDASDASD");

        Handles.EndGUI();

        if (plantArea.Contains(Event.current.mousePosition) || true) {

            Vector3 pos;

            Vector3 mouseWorld = WorldPos(Event.current.mousePosition);
            Debug.Log("Mouse world: " + mouseWorld);
            Vector3 meshLocalMouse = editor.Generator.Meshes.InverseTransformPoint(mouseWorld);


            LBranch closest = editor.Generator.Tree.GetClosestChildNode(p.position, meshLocalMouse, out pos);
            Debug.Log("FOund raw: " + pos);
            pos = editor.Generator.Meshes.TransformPoint(pos);
            Debug.Log("Found world: " + pos);
            GameObject.FindGameObjectWithTag("Finish").transform.position = pos;
        } else {
            Debug.Log("not in area");
        }


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