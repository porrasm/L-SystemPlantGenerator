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
    public Color Color = Color.white;
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