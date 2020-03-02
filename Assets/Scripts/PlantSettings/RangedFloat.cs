using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

[Serializable]
public class RangedFloat : ISetting {
    #region fields
    [SerializeField]
    private float targetValue;
    [SerializeField]
    private ProbabilityDistributionEditor distribution;

    public float TargetValue { get => targetValue; set => targetValue = value; }
    public ProbabilityDistributionEditor Distribution { get => distribution; }
    public float MinValue { get => targetValue - distribution.Min; }
    public float MaxValue { get => targetValue + distribution.Max; }
    #endregion

    public RangedFloat(float value = 0, float min = 0, float max = 0, bool enabled = true) {

        targetValue = value;

        if (max > min) {
            distribution = new ProbabilityDistributionEditor(ProbabilityDistributionEditor.DEFAULT_ACCURACY, min, max);
            distribution.Enabled = true;
        } else {
            distribution = new ProbabilityDistributionEditor();
            distribution.Enabled = false;
        }

        if (!enabled) {
            distribution.Enabled = false;
        }
    }

    //public void SetTargetValue(float newValue) {
    //    float diff = newValue - targetValue;
    //    targetValue = newValue;
    //    distribution.ShiftRange(diff);
    //}

    public float Value {
        get {
            return targetValue + distribution.GetSeededFloat();
        }
    }

    public void Validate() {
        distribution.Validate();
    }

    public static implicit operator RangedFloat(float value) {
        return new RangedFloat(value);
    }
}