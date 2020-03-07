using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

[Serializable]
public class LineState {

    #region fields
    public float Angle = 0;
    public float Orientation = 0;
    public float Width = 0.05f;
    public float CurrentLength = 0;
    public float NextLength = 0.25f;
    public SColor Color = new SColor(1, 1, 1, 1);
    #endregion

    public LineState Copy() {
        LineState copy = new LineState();

        copy.Angle = Angle;
        copy.Orientation = Orientation;
        copy.Width = Width;
        copy.CurrentLength = CurrentLength;
        copy.NextLength = NextLength;
        copy.Color = Color;

        return copy;
    }

    public void AddAngle(int mult) {
        Orientation += mult * Angle;
    }

    public Vector2 GetOrientationDirection() {
        Vector2 dir = Vector2.up;
        return (Quaternion.AngleAxis(Orientation, Vector3.forward) * dir).normalized;
    }
    public Vector2 GetEndPos(Vector2 lastPos) {
        return lastPos + GetOrientationDirection() * CurrentLength;
    }

    #region parameters
    public void ExecuteParameterCommands(StringCommand command) {
        foreach (string param in command.GetParameters()) {
            ExecuteParameterCommand(param);
        }
    }
    private void ExecuteParameterCommand(string uncleanCommand) {
        string command = Regex.Replace(uncleanCommand, @"\s+", "");

        if (command.Contains("=")) {
            string[] split = command.Split('=');
            ParamCalculations(split[0], split[1]);
        }
    }

    // Unityeditor namespcae wont work in build, replace expressionevaluator
    private void ParamCalculations(string definition, string exp) {

        if (definition.Equals("angle")) {
            string cleaned = exp.Replace(definition, Angle.ToString());
            float value;
            if (ExpressionEvaluator.Evaluate(cleaned, out value)) {
                Angle = value;
            }
        }
    }
    #endregion
}

[Serializable]
public struct SColor {
    public float r;
    public float g;
    public float b;
    public float a;
    public SColor(float r, float g, float b, float a) {
        this.r = r;
        this.g = g;
        this.b = b;
        this.a = a;
    }
    public Color Color {
        get {
            return new Color(r, g, b, a);
        }
    }
    public static implicit operator SColor(Color c) {
        return new SColor(c.r, c.g, c.b, c.a);
    }
}