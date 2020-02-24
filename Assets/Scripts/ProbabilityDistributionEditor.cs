using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProbabilityDistributionEditor {

    #region fields
    [SerializeField]
    private int accuracy;
    [SerializeField]
    private float min, max;

    [SerializeField]
    private AnimationCurve curve;

    [SerializeField]
    private bool enabled;

    private float lastTargetValue;
    private float offset;

    public const float MAX_VALUE = 1;

    private bool prepared;
    private ProbabilityDistribution distribution;

    public int Accuracy {
        get => accuracy; set {
            if (value < 1) {
                throw new System.Exception("Accuracy too small");
            }
            accuracy = value;
        }
    }
    public AnimationCurve Curve { get => curve; set => curve = value; }
    public float Min { get => min; }
    public float Max { get => max; }
    public bool Enabled { get => enabled; set => enabled = value; }

    public const int DEFAULT_ACCURACY = 100;

    public float AdjustedMin {
        get {
            return min + offset;
        }
    }
    public float AdjustedMax {
        get {
            return max + offset;
        }
    }
    #endregion

    #region initialization
    public ProbabilityDistributionEditor() {
        this.Accuracy = DEFAULT_ACCURACY;
        distribution = new ProbabilityDistribution(this);
        curve = new AnimationCurve();
        SetRange(-1, 1);
        ResetCurve();
    }
    public ProbabilityDistributionEditor(int accuracy, float min, float max) {
        this.Accuracy = accuracy;
        distribution = new ProbabilityDistribution(this);
        curve = new AnimationCurve();
        SetRange(min, max);
        ResetCurve();
    }

    public void SetRange(float min, float max) {
        if (max <= min) {
            throw new System.Exception("max <= min");
        }

        prepared = false;
        distribution.Clear();

        this.min = min;
        this.max = max;
    }

    public void ResetCurve() {
        Clear();

        Curve.AddKey(min, 0);
        Curve.AddKey(MidPoint, MAX_VALUE);
        Curve.AddKey(max, 0);
    }
    private void Clear() {
        while (Curve.length > 0) {
            Curve.RemoveKey(0);
        }
    }
    #endregion

    public void Validate() {

        Keyframe[] keys = Curve.keys;

        float maxFoundValue = -1;
        for (int i = 0; i < keys.Length; i++) {

            if (keys[i].value < 0) {
                keys[i].value = 0;
            } else if (keys[i].value > MAX_VALUE) {
                keys[i].value = MAX_VALUE;
            }

            if (keys[i].value > maxFoundValue) {
                maxFoundValue = keys[i].value;
            }
        }

        float multiplier = MAX_VALUE / maxFoundValue;

        for (int i = 0; i < keys.Length; i++) {
            keys[i].value *= multiplier;
        }

        keys[0].time = AdjustedMin;
        keys[keys.Length - 1].time = AdjustedMax;

        Curve.keys = keys;
    }

    public void SetOffset(float newOffset) {
        float diff = newOffset - this.offset;
        this.offset = newOffset;

        ShiftRange(diff);
    }

    private void ShiftRange(float amount) {

        Keyframe[] keys = Curve.keys;

        for (int i = 0; i < keys.Length; i++) {
            keys[i].time += amount;
        }

        Curve.keys = keys;
    }

    private float MidPoint {
        get {
            return max - (max - min) * 0.5f + offset;
        }
    }

    public void CalculateDistribution() {
        distribution.Calculate();
    }
    public float GetSeededFloat() {

        if (!enabled) {
            return 0;
        } else if (!prepared) {
            throw new System.Exception("Not prepared");
        }

        return distribution.GetSeededFloat();
    }
}