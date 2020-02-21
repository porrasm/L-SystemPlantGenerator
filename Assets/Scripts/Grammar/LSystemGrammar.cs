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
    private List<CharacterDefinition> characterDefinitions;

    public List<CharacterDefinition> CharacterDefinitions { get => characterDefinitions; }

    private Dictionary<char, CharacterDefinition> characters;
    #endregion

    public LSystemGrammar() {
        characterDefinitions = new List<CharacterDefinition>();
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
        HashSet<char> used = new HashSet<char>();
        HashSet<char> aliases = new HashSet<char>();

        foreach (CharacterDefinition r in characterDefinitions) {
            if (!used.Add(r.Character)) {
                Debug.LogError("Rule character was used multiple times");
            }
            r.Validate();

            if (r.Type == CharacterDefinition.CharacterType.Alias) {
                if (!aliases.Add(r.Character)) {
                    throw new Exception("Alias character cannot be " + r.Character);
                }
            }
        }
        foreach (CharacterDefinition r in characterDefinitions) {
            if (r.Type == CharacterDefinition.CharacterType.Alias) {
                foreach (char a in aliases) {
                    if (r.Alias.Contains(a)) {
                        throw new Exception("Alias cannot contain other aliases. Alias '" + r.Alias + "' contained '" + a + "'");
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
            iteration = newIteration;
            Debug.Log("Line count: " + lineCount);
        }

        iteration = TransformAliases(iteration);

        return iteration;
    }
    private void PrepareDictionary() {
        characters = new Dictionary<char, CharacterDefinition>();

        foreach (CharacterDefinition c in CharacterDefinitions) {
            characters.Add(c.Character, c);
        }
    }

    private string Iterate(string iteration, out int lineCount) {
        StringBuilder newIteration = new StringBuilder();

        lineCount = 0;
        foreach (char c in iteration) {
            CharacterDefinition rule;
            if (RuleByCharacter(c, out rule)) {

                string ruleString;
                int ruleLineCount;
                rule.GetRule(out ruleString, out ruleLineCount);

                newIteration.Append(ruleString);
                lineCount += ruleLineCount;
            } else {
                if (c == 'f') {
                    lineCount++;
                }
                newIteration.Append(c);
            }
        }

        return newIteration.ToString();
    }
    private string TransformAliases(string iteration) {
        StringBuilder newIteration = new StringBuilder();

        foreach (char c in iteration) {
            CharacterDefinition r;
            if (RuleByCharacter(c, out r)) {
                if (r.Type == CharacterDefinition.CharacterType.Alias) {
                    newIteration.Append(r.Alias);
                } else {
                    newIteration.Append(c);
                }
            } else {
                newIteration.Append(c);
            }
        }

        return newIteration.ToString();
    }

    private bool RuleByCharacter(char c, out CharacterDefinition rule) {
        if (characters.ContainsKey(c)) {
            rule = characters[c];
            return true;
        }
        rule = null;
        return false;
    }

    private void AdjustProbabilities() {
        foreach (CharacterDefinition rule in characterDefinitions) {
            rule.AdjustProbabilities();
        }
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
