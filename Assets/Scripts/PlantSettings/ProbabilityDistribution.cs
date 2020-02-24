using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProbabilityDistribution {

    #region fields
    private ProbabilityDistributionEditor editor;

    private float distanceStep;
    private double cumulativeAmount;
    private double[] cumulativeDistribution;
    #endregion

    public ProbabilityDistribution(ProbabilityDistributionEditor editor) {
        this.editor = editor;
    }

    public void Clear() {
        cumulativeDistribution = null;
    }  

    public void Calculate() {
        cumulativeDistribution = new double[editor.Accuracy];

        cumulativeAmount = 0;
        distanceStep = (editor.Max - editor.Min) / editor.Accuracy;

        for (int i = 0; i < editor.Accuracy; i++) {

            float time = editor.Min + i * distanceStep - editor.Offset;
            float evaluated = editor.Curve.Evaluate(time);

            if (evaluated < 0) {
                evaluated = 0;
            } else if (evaluated > ProbabilityDistributionEditor.MAX_VALUE) {
                evaluated = ProbabilityDistributionEditor.MAX_VALUE;
            }

            cumulativeAmount += evaluated;
            cumulativeDistribution[i] = cumulativeAmount;
        }
    }

    public float GetSeededFloat() {
        double randomCumulativePoint = RNG.SeededFloat * cumulativeAmount;

        double foundValue = 0;

        int i;
        for (i = 0; i < editor.Accuracy; i++) {
            if (randomCumulativePoint < cumulativeDistribution[i]) {
                foundValue = editor.Min + i * distanceStep;
                break;
            }
        }

        double minValue = i > 0 ? foundValue - 0.5f * distanceStep : foundValue;
        double maxValue = i < editor.Accuracy - 1 ? foundValue + 0.5f * distanceStep : foundValue;

        return RNG.SeededRange((float)minValue, (float)maxValue);
    }
}