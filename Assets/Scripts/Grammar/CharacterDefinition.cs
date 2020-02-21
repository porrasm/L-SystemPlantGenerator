using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class CharacterDefinition : ISetting {

    #region fields
    [SerializeField]
    private char character;

    [SerializeField]
    private List<ProbabilityRule> rules;

    [SerializeField]
    private CharacterType type;

    [SerializeField]
    private string alias;

    public char Character { get => character; set => character = value; }
    public List<ProbabilityRule> Rules { get => rules; set => rules = value; }
    public CharacterType Type { get => type; set => type = value; }
    public string Alias { get => alias; set => alias = value; }

    public enum CharacterType {
        Variable,
        Alias
    }
    #endregion

    public CharacterDefinition() {
        rules = new List<ProbabilityRule>();
    }

    public ProbabilityRule DefaultRule {
        get {
            ProbabilityRule rule = new ProbabilityRule();
            rule.Rule = "" + character;
            return rule;
        }
    }

    public void AdjustProbabilities() {
        double total = 0;

        foreach (ProbabilityRule rule in rules) {
            total += rule.Probability;
        }

        if (total == 0) {
            double range = 1.0 / rules.Count;
            for (int i = 0; i < rules.Count; i++) {
                rules[i].ProbabilityRange = (i + 1) * range;
            }
            return;
        }

        double multiplier = 1 / total;
        double rangeCovered = 0;

        foreach (ProbabilityRule rule in rules) {
            rangeCovered += rule.Probability * multiplier;
            rule.ProbabilityRange = rangeCovered;
        }
    }

    public void GetRule(out string rule, out int lineCount) {

        if (TransformAlias(out rule, out lineCount)) {
            return;
        }

        if (rules.Count == 0) {
            rule = "";
            lineCount = 0;
            return;
        }

        float rangePos = RNG.SeededFloat;

        foreach (ProbabilityRule r in rules) {
            if (rangePos < r.ProbabilityRange) {
                lineCount = LSystemGrammar.LineCount(r.Rule);
                rule = r.Rule;
                return;
            }
        }

        lineCount = LSystemGrammar.LineCount(rules[0].Rule);
        rule = rules[0].Rule;
    }
    public bool TransformAlias(out string rule, out int lineCount) {
        if (type == CharacterType.Alias) {
            rule = Alias;
            lineCount = LSystemGrammar.LineCount(Alias);
            return true;
        } else {
            rule = "";
            lineCount = 0;
            return false;
        }
    }

    public void RemoveRule(int index) {
        rules.RemoveAt(index);
        if (rules.Count == 0) {
            rules.Add(DefaultRule);
        }
    }

    public void Validate() {
        character = Char.ToLower(character);
        foreach (ProbabilityRule r in rules) {
            r.Rule = r.Rule.ToLower();
        }
    }
}