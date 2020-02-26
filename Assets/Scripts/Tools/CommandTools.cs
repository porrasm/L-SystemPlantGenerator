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
        string command;
        if (longString[index] == '{') {
            command = StringUntilChar(longString, index + 1, '}');
            index += command.Length + 2;
        } else {
            command = "" + longString[index];
            index++;
            if (longString.Length - 1 > index + 1) {
                if (longString[index + 1] == '(') {
                    command += StringUntilChar(longString, index + 1, ')');
                    index += command.Length + 2;
                }
            }
        }
        return SplitCommand(command);
    }
    private static string StringUntilChar(string s, int i, char c) {
        int endIndex = i;
        while (s[endIndex] != c) {
            endIndex++;
        }
        return s.Substring(i, endIndex - i);
    }

    public static StringCommand SplitCommand(string command) {
        StringCommand c = new StringCommand();
        string[] paramSplit = command.Split('(');
        c.Command = paramSplit[0];
        if (paramSplit.Length > 1) {
            c.ParamString = paramSplit[1].Split(')')[0];
        } else {
            c.ParamString = "";
        }
        return c;
    }
    
}
