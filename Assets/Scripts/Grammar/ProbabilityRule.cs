using System;
using UnityEngine;

[Serializable]
public class ProbabilityRule {
    [Range(0, 1)]
    [Tooltip("The probability that this rule will be used. Leave all probabilities to zero to distribute them evenly.")]
    public float Probability;
    public string Rule;
    [NonSerialized]
    public double ProbabilityRange;
}