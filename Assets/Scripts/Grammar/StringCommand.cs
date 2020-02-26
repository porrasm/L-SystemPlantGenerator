
using UnityEngine;

public struct StringCommand {
    public string Command;
    public CommandType Type;

    public enum CommandType {
        Command,
        CommandParameter
    }

    public override string ToString() {
        if (Type == CommandType.Command) {
            if (Command.Length > 1) {
                return "{" + Command + "}";
            } else {
                return Command;
            }
        }
        return "[" + Command + "]";
    }
    public string[] GetParameters() {
        string[] split = Command.Split(',');
        if (split.Length == 1 && split[0].Length == 0) {
            return new string[0];
        }
        return split;
    }
}
