using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

[Serializable]
public class LSystemGrammar : ISetting {

    #region fields
    private HashSet<char> reservedCharacters;

    [SerializeField]
    private List<Rule> variableRules;
    #endregion

    public LSystemGrammar() {
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
        if (variableRules.Count == 0) {
            Debug.LogError("Variable rule count was 0");
        }
        foreach (Rule r in variableRules) {
            if (!used.Add(r.Character)) {
                Debug.LogError("Rule character was used multiple times");
            }
        }
    }
    #endregion

    #region string transformation
    public string PerformIterations(string axiom, int iterations) {
        AdjustProbabilities();

        string iteration = axiom.ToLower();
        for (int i = 0; i < iterations; i++) {
            iteration = Iterate(iteration);
        }

        return iteration;
    }
    private string Iterate(string iteration) {
        StringBuilder newIteration = new StringBuilder();

        foreach (char c in iteration) {
            Rule rule;
            if (RuleByCharacter(c, out rule)) {
                newIteration.Append(rule.GetRule());
            } else {
                newIteration.Append(c);
            }
        }

        return newIteration.ToString();
    }
    private bool RuleByCharacter(char c, out Rule rule) {
        foreach (Rule r in variableRules) {
            if (r.Character == c) {
                rule = r;
                return true;
            }
        }
        rule = null;
        return false;
    }

    private void AdjustProbabilities() {
        foreach (Rule rule in variableRules) {
            rule.AdjustProbabilities();
        }
    }
    #endregion
}
