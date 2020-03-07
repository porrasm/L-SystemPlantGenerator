using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeCreator {

    #region fields
    private PlantSettings settings;
    private string treeString;
    private Plant plant;
    private int current;
    #endregion

    public TreeCreator(PlantSettings settings) {
        this.settings = settings;
    }

    public Plant CreatePlant(string treeString) {

        this.treeString = treeString;

        Logger.Print("Result: " + treeString.Length);


        plant = new Plant(settings.Properties.ToLineState());
        current = 0;

        List<StringCommand> commands = CommandTools.GetCommands(treeString);

        foreach (StringCommand command in commands) {
            ExecuteCommand(command);
        }

        Plant saved = plant;
        plant = null;
        treeString = null;
        return saved;
    }

    private void ExecuteCommand(StringCommand s) {

        // Handle state parameters
        if (s.Type == StringCommand.CommandType.Command) {
            if (s.Command.Equals("f")) {
                AddVariationToCurrent();
                current = plant.AppendAfter(current);
            }
            if (s.Command.Equals("+")) {
                plant.GetPart(current).State.AddAngle(1);
                AddVariationToCurrent();
            }
            if (s.Command.Equals("-")) {
                plant.GetPart(current).State.AddAngle(-1);
                AddVariationToCurrent();
            }
            if (s.Command.Equals("(")) {
                current = plant.AppenUnder(current);
            }
            if (s.Command.Equals(")")) {
                current = plant.GetPart(current).Parent;
            }
        } else if (s.Type == StringCommand.CommandType.CommandParameter) {
#if UNITY_EDITOR
            plant.GetPart(current).State.ExecuteParameterCommands(s);
#endif
        }
    }
    private void AddVariationToCurrent() {
        LineState state = plant.GetPart(current).State;
        state.Width += settings.Properties.LineWidthVariance.GetSeededFloat();
        state.Orientation += settings.Properties.AngleVariance.GetSeededFloat();
        state.CurrentLength = state.NextLength + settings.Properties.LineLengthVariance.GetSeededFloat();
    }
}