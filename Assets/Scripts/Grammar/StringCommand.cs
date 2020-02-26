
using UnityEngine;

public struct StringCommand {
    public string Command;
    public string ParamString;

    public override string ToString() {
        string parameters = "";

        if (ParamString.Length > 0) {
            parameters = "(" + ParamString + ")";
        }

        if (Command.Length > 1) {
            return "{" + Command + parameters + "}";
        } else {
            return Command + parameters;
        }
    }
    public string[] GetParameters() {
        string[] split = ParamString.Split(',');
        if (split.Length == 1 && split[0].Length == 0) {
            return new string[0];
        }
        return split;
    }
}
