using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

public static class InspectorGUI {

    #region integer
    public static int IntegerField(string label, int previousValue, int min = 0, int max = 0) {

        GUILayout.BeginHorizontal();
        GUILayout.Label(label);
        int newValue = EditorGUILayout.IntField(previousValue);
        GUILayout.EndHorizontal();

        return Clamp(newValue, min, max);
    }
    #endregion

    #region float
    public static float FloatField(string label, float previousValue, float min = 0, float max = 0) {

        GUILayout.BeginHorizontal();
        GUILayout.Label(label);
        float newValue = EditorGUILayout.FloatField(previousValue);
        GUILayout.EndHorizontal();

        return Clamp(newValue, min, max);
    }
    public static float FloatSlider(string label, float previousValue, float min = 0, float max = 0) {

        GUILayout.BeginHorizontal();
        GUILayout.Label(label);
        float newValue = EditorGUILayout.Slider(previousValue, min, max);
        GUILayout.EndHorizontal();

        return Clamp(newValue, min, max);
    }
    #endregion

    #region helpers
    private static int Clamp(int val, int min, int max) {
        if (max <= min) {
            return val;
        } else if (val > max) {
            return max;
        } else if (val < min) {
            return min;
        } else {
            return val;
        }
    }
    private static float Clamp(float val, float min, float max) {
        if (max <= min) {
            return val;
        } else if (val > max) {
            return max;
        } else if (val < min) {
            return min;
        } else {
            return val;
        }
    }
    #endregion
}