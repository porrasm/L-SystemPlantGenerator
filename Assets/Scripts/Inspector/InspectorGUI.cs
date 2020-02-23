﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

public static class InspectorGUI {

    public static void CreateArea(string area, int dividerWidth = 10, int spaceWidth = 0, bool boxed = true) {
        GUILayout.Space(dividerWidth);
        GUILayout.Label(area);
        GUILayout.Space(spaceWidth);
        if (boxed) {
            GUILayout.BeginVertical("box");
        }
    }
    public static void CreateBox() {
        GUILayout.BeginVertical("box");
    }
    public static void EndArea() {
        GUILayout.EndVertical();
    }

    public static bool CreateFoldedArea(string area, ref bool open, int dividerWidth = 0, int spaceWidth = 10) {

        open = EditorGUILayout.Foldout(open, area);

        if (open) {
            EditorGUI.indentLevel++;
            GUILayout.Space(dividerWidth);
        }

        return open;
    }
    public static void EndFoldedArea() {
        EditorGUI.indentLevel--;
    }

    #region integer
    public static int IntegerField(string label, int previousValue, int min = 0, int max = 0) {

        GUILayout.BeginHorizontal();
        if (label.Length > 0) {
            GUILayout.Label(label);
        }
        int newValue = EditorGUILayout.IntField(previousValue);
        GUILayout.EndHorizontal();

        return Clamp(newValue, min, max);
    }
    public static int IntegerSlider(string label, int previousValue, int min = 0, int max = 0) {

        GUILayout.BeginHorizontal();
        if (label.Length > 0) {
            GUILayout.Label(label);
        }
        int newValue = EditorGUILayout.IntSlider(previousValue, min, max);
        GUILayout.EndHorizontal();

        return Clamp(newValue, min, max);
    }
    #endregion

    #region float
    public static float FloatField(string label, float previousValue, float min = 0, float max = 0) {

        GUILayout.BeginHorizontal();
        if (label.Length > 0) {
            GUILayout.Label(label);
        }
        float newValue = EditorGUILayout.FloatField(previousValue);
        GUILayout.EndHorizontal();

        return Clamp(newValue, min, max);
    }
    public static float FloatSlider(string label, float previousValue, float min = 0, float max = 0) {

        GUILayout.BeginHorizontal();
        if (label.Length > 0) {
            GUILayout.Label(label);
        }
        float newValue = EditorGUILayout.Slider(previousValue, min, max);
        GUILayout.EndHorizontal();

        return Clamp(newValue, min, max);
    }
    #endregion

    #region string
    public static string TextField(string label, string previousValue, int maxLength = int.MaxValue) {

        GUILayout.BeginHorizontal();
        if (label.Length > 0) {
            GUILayout.Label(label);
        }
        string newValue = GUILayout.TextField(previousValue);
        GUILayout.EndHorizontal();

        if (newValue == null) {
            return "";
        }
        if (newValue.Length > maxLength) {
            return newValue.Substring(0, maxLength);
        }

        return newValue;
    }
    public static string TextArea(string label, string previousValue, int maxLength = int.MaxValue, bool horizontal = false) {

        if (horizontal) {
            GUILayout.BeginHorizontal();
        }

        if (label.Length > 0) {
            GUILayout.Label(label);
        }
        string newValue = GUILayout.TextArea(previousValue);

        if (horizontal) {
            GUILayout.EndHorizontal();
        }

        if (newValue == null) {
            return "";
        }
        if (newValue.Length > maxLength) {
            return newValue.Substring(0, maxLength);
        }

        return newValue;
    }
    #endregion

    #region other types
    public static bool BoolField(string label, bool previousValue) {

        GUILayout.BeginHorizontal();
        if (label.Length > 0) {
            GUILayout.Label(label);
        }
        bool newValue = EditorGUILayout.Toggle(previousValue);
        GUILayout.EndHorizontal();

        return newValue;
    }

    public static Color ColorField(string label, Color previousValue) {

        GUILayout.BeginHorizontal();
        if (label.Length > 0) {
            GUILayout.Label(label);
        }
        Color newValue = EditorGUILayout.ColorField(previousValue);
        GUILayout.EndHorizontal();

        return newValue;
    }

    public static void DistributionEditor(string label, ProbabilityDistribution dist) {

        if (label.Length > 0) {
            GUILayout.Label(label);
        }

        if (dist.Enabled) {

            CreateBox();
            dist.Accuracy = IntegerField("Accuracy", dist.Accuracy, 1);
            float min = FloatField("Min value", dist.Min);
            float max = FloatField("Min value", dist.Max);

            GUILayout.Space(10);
            dist.Curve = EditorGUILayout.CurveField(dist.Curve);
            dist.Validate();

            if (GUILayout.Button("Reset curve")) {
                dist.ResetCurve();
            }
            if (GUILayout.Button("Disable distribution")) {
                dist.Enabled = false;
            }

            EndArea();

            if (max > min) {
                dist.SetRange(min, max);
            }
        } else {
            if (GUILayout.Button("Enable distribution")) {
                dist.Enabled = true;
            }
        }
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