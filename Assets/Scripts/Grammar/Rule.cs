using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Rule : ISetting {

    #region fields
    
    [SerializeField]
    private char character;
    public char Character { get => character; set => character = value; }

    [SerializeField]
    private List<ProbabilityRule> Rules;
    #endregion

    public void AdjustProbabilities() {
        double total = 0;

        foreach (ProbabilityRule rule in Rules) {
            total += rule.Probability;
        }

        if (total == 0) {
            double range =  1.0 / Rules.Count;
            for (int i = 0; i < Rules.Count; i++) {
                Rules[i].ProbabilityRange = (i+1) * range;
            }
            return;
        }

        double multiplier = 1 / total;
        double rangeCovered = 0;

        foreach (ProbabilityRule rule in Rules) {
            rangeCovered += rule.Probability * multiplier;
            rule.ProbabilityRange = rangeCovered;
        }
    }

    public string GetRule() {

        float rangePos = UnityEngine.Random.value;

        foreach (ProbabilityRule rule in Rules) {
            if (rangePos < rule.ProbabilityRange) {
                return rule.Rule;
            }
        }

        return Rules[0].Rule;
    }

    public void Validate() {
        character = Char.ToLower(character);
    }
}