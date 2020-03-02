using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

[Serializable]
public class LSystemGrammar : ISetting {

    #region fields
    private const int LINE_LIMIT = 16383 - 4;

    private HashSet<char> reservedCharacters;

    [SerializeField]
    private List<CommandDefinition> characterDefinitions;

    public List<CommandDefinition> CharacterDefinitions { get => characterDefinitions; }

    private Dictionary<string, CommandDefinition> commands;
    #endregion

    public LSystemGrammar() {
        characterDefinitions = new List<CommandDefinition>();
        ReserveCharacters();
    }
    private void ReserveCharacters() {
        reservedCharacters = new HashSet<char>();

        reservedCharacters.Add('f');
        reservedCharacters.Add('+');
        reservedCharacters.Add('-');
        reservedCharacters.Add('(');
        reservedCharacters.Add(')');
        reservedCharacters.Add('$');
    }

    #region validation
    public void Validate() {
        ValidateVariables();
    }
    private void ValidateVariables() {
        HashSet<string> used = new HashSet<string>();
        HashSet<string> aliases = new HashSet<string>();

        foreach (CommandDefinition r in characterDefinitions) {
            if (r.Command == null || r.Command.Length == 0) {
                Debug.LogWarning("Command was empty");
            }
            if (!used.Add(r.Command)) {
                Debug.LogError("Command was used multiple times: " + r.Command);
            }
            r.Validate();

            if (r.Type == CommandDefinition.CommandType.Alias) {
                if (!aliases.Add(r.Command)) {
                    throw new Exception("Alias character cannot be " + r.Command);
                }
            }
        }

        // Must rethink after using multichar strings warpped in {} as commands
        foreach (CommandDefinition r in characterDefinitions) {
            if (r.Type == CommandDefinition.CommandType.Alias) {
                foreach (string a in aliases) {
                    if (r.Alias.Contains(a)) {
                        throw new Exception("Alias cannot contain other aliases. Alias '" + r.Alias + "' contained '" + a + "'");
                    }
                    if (r.Alias.Contains("{" + a + "}")) {
                        throw new Exception("Alias cannot contain other aliases. Alias '" + r.Alias + "' contained '{" + a + "}'");
                    }
                }
            }
        }
    }
    #endregion

    #region string transformation
    public string PerformIterations(string axiom, int iterations) {

        Validate();
        PrepareDictionary();

        if (axiom == null || axiom.Length == 0) {
            return "";
        }

        AdjustProbabilities();

        string iteration = axiom.ToLower();

        for (int i = 0; i < iterations; i++) {
            int lineCount;
            string newIteration = Iterate(iteration, out lineCount);

            newIteration = TransformAliases(newIteration);
            if (lineCount > LINE_LIMIT) {
                Debug.LogWarning("Too many iterations: " + iterations + ". Succesfully performed " + i + " iterations.");
                break;
            }

            Debug.Log("Line count: " + lineCount);
            iteration = newIteration;
        }

        iteration = TransformAliases(iteration);

        return iteration;
    }
    private void PrepareDictionary() {
        commands = new Dictionary<string, CommandDefinition>();

        foreach (CommandDefinition c in CharacterDefinitions) {
            commands.Add(c.Command, c);
        }
    }

    private string Iterate(string iteration, out int lineCount) {
        StringBuilder newIteration = new StringBuilder();

        List<StringCommand> commands = CommandTools.GetCommands(iteration);

        // state params are not saved after iterations, fix

        lineCount = 0;
        foreach (StringCommand command in commands) {

            if (command.Type == StringCommand.CommandType.Command) {
                CommandDefinition rule;
                if (RuleByString(command.Command, out rule)) {
                    string ruleString;
                    int ruleLineCount;
                    rule.GetRule(out ruleString, out ruleLineCount);

                    newIteration.Append(ruleString);
                    lineCount += ruleLineCount;
                } else {
                    if (command.Command.Equals("f")) {
                        lineCount++;
                    }
                    newIteration.Append(command);
                }
            } else {
                newIteration.Append(command);
            }
        }

        return newIteration.ToString();
    }

    private string TransformAliases(string iteration) {
        StringBuilder newIteration = new StringBuilder();

        Debug.Log("Transform aliases: " + iteration);
        List<StringCommand> commands = CommandTools.GetCommands(iteration);

        foreach (StringCommand command in commands) {

            if (command.Type == StringCommand.CommandType.Command) {
                CommandDefinition r;
                if (RuleByString(command.Command, out r)) {
                    if (r.Type == CommandDefinition.CommandType.Alias) {
                        newIteration.Append(r.Alias);
                    } else {
                        newIteration.Append(command);
                    }
                } else {
                    newIteration.Append(command);
                }
            } else {
                newIteration.Append(command);
            }
        }

        Debug.Log("Transformed to: " + newIteration.ToString());
        return newIteration.ToString();
    }

    private bool RuleByString(string s, out CommandDefinition rule) {
        if (commands.ContainsKey(s)) {
            rule = commands[s];
            return true;
        }

        rule = null;
        return false;
    }

    private void AdjustProbabilities() {
        foreach (CommandDefinition rule in characterDefinitions) {
            rule.AdjustProbabilities();
        }
    }
    #endregion

    #region plant parsing
    public void ParseFromTree(string tree, int targetIterations) {
        List<string> rules = new List<string>();

        throw new NotImplementedException("Possible feature, difficult to implement");
    }
    #endregion

    public static int LineCount(string rule) {
        int count = 0;
        foreach (char c in rule) {
            if (c == GrammarChars.LINE_CHAR) {
                count++;
            }
        }
        return count;
    }
}
