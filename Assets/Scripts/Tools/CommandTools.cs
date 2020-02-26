using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public static class CommandTools {

    public static List<StringCommand> GetCommands(string s) {
        int index = 0;
        List<StringCommand> commands = new List<StringCommand>();
        while (index < s.Length) {
            StringCommand command = GetCommand(s, ref index);
            commands.Add(command);
        }
        return commands;
    }
    public static StringCommand GetCommand(string longString, ref int index) {

        StringCommand command = new StringCommand();

        // longer command
        if (longString[index] == '{') {
            command.Command = StringUntilChar(longString, index + 1, '}');
            command.Type = StringCommand.CommandType.Command;
            index += command.Command.Length + 2;
        } else if (longString[index] == '[') {
            command.Command = StringUntilChar(longString, index + 1, ']');
            command.Type = StringCommand.CommandType.CommandParameter;
            index += command.Command.Length + 2;
        } else {
            command.Command = "" + longString[index];
            command.Type = StringCommand.CommandType.Command;
            index++;
        }

        return command;
    }

    private static string StringUntilChar(string s, int i, char c) {
        int endIndex = i;
        while (s[endIndex] != c) {
            endIndex++;
        }
        return s.Substring(i, endIndex - i);
    } 
}
