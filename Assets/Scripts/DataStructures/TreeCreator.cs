﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeCreator {

    #region fields
    private PlantSettings settings;
    private string treeString;
    private LBranch node;
    #endregion

    public TreeCreator(PlantSettings settings) {
        this.settings = settings;
    }

    public LBranch CreateTree(string treeString) {

        this.treeString = treeString;

        Logger.Print("Result: " + treeString.Length);

        LBranch tree = new LBranch(settings.Properties.ToLineState());
        node = tree;

        List<StringCommand> commands = CommandTools.GetCommands(treeString);

        foreach (StringCommand command in commands) {
            ExecuteCommand(command);
        }

        node = null;
        return tree;
    }
    private void ExecuteCommand(StringCommand s) {

        // Handle state parameters
        if (s.Type == StringCommand.CommandType.Command) {
            if (s.Command.Equals("f")) {
                AddVariationToCurrent();
                node = node.Append();
            }
            if (s.Command.Equals("+")) {
                node.State.Orientation += node.State.Angle;
                AddVariationToCurrent();
            }
            if (s.Command.Equals("-")) {
                node.State.Orientation -= node.State.Angle;
                AddVariationToCurrent();
            }
            if (s.Command.Equals("(")) {
                node = node.AddChild();
            }
            if (s.Command.Equals(")")) {
                node = node.Parent;
            }
        } else if (s.Type == StringCommand.CommandType.CommandParameter) {
#if UNITY_EDITOR
            node.State.ExecuteParameterCommands(s);
#endif
        }
    }
    private void AddVariationToCurrent() {
        node.State.Width += settings.Properties.LineWidthVariance.GetSeededFloat();
        node.State.Orientation += settings.Properties.AngleVariance.GetSeededFloat();
        node.State.CurrentLength = node.State.NextLength + settings.Properties.LineLengthVariance.GetSeededFloat();
    }
}