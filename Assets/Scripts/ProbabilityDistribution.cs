using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProbabilityDistribution {

    #region fields
    [SerializeField]
    private int accuracy;
    [SerializeField]
    private float min, max;

    [SerializeField]
    private AnimationCurve curve;

    [SerializeField]
    private bool enabled;

    private const float maxValue = 1;

    private bool prepared;
    private float distanceStep;
    private double cumulativeAmount;
    private double[] cumulativeDistribution;

    public int Accuracy { get => accuracy; set {
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
    #endregion

    #region initialization
    public ProbabilityDistribution() {
        this.Accuracy = 100;
        curve = new AnimationCurve();
        SetRange(-1, 1);
        ResetCurve();
    }
    public ProbabilityDistribution(int accuracy, float min, float max) {
        this.Accuracy = accuracy;
        curve = new AnimationCurve();
        SetRange(min, max);
        ResetCurve();
    }

    public void SetRange(float min, float max) {
        if (max <= min) {
            throw new System.Exception("max <= min");
        }

        prepared = false;
        cumulativeDistribution = null;

        this.min = min;
        this.max = max;
    }
    public void ResetCurve() {
        Clear();

        Curve.AddKey(min, 0);
        Curve.AddKey(MidPoint, maxValue);
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
            } else if (keys[i].value > maxValue) {
                keys[i].value = maxValue;
            }

            if (keys[i].value > maxFoundValue) {
                maxFoundValue = keys[i].value;
            }
        }

        float multiplier = maxValue / maxFoundValue;

        for (int i = 0; i < keys.Length; i++) {
            keys[i].value *= multiplier;
        }

        keys[0].time = min;
        keys[keys.Length - 1].time = max;

        Curve.keys = keys;
    }

    public void Prepare() {

        cumulativeDistribution = new double[accuracy];

        cumulativeAmount = 0;
        distanceStep = (max - min) / accuracy;

        for (int i = 0; i < accuracy; i++) {

            float time = this.min + i * distanceStep;
            float evaluated = Curve.Evaluate(time);

            if (evaluated < 0) {
                evaluated = 0;
            } else if (evaluated > maxValue) {
                evaluated = maxValue;
            }

            cumulativeAmount += evaluated;
            cumulativeDistribution[i] = cumulativeAmount;
        }

        prepared = true;
    }

    public float GetSeededFloat() {
        if (!enabled) {
            return 0;
        }
        if (!prepared) {
            throw new System.Exception("Distribution not prepared");
        }

        double randomCumulativePoint = RNG.SeededFloat * cumulativeAmount;

        double foundValue = 0;

        int i;
        for (i = 0; i < accuracy; i++) {
            if (randomCumulativePoint < cumulativeDistribution[i]) {
                foundValue = this.min + i * distanceStep;
                break;
            }
        }

        double min = i > 0 ? foundValue - 0.5f * distanceStep : foundValue;
        double max = i < accuracy - 1 ? foundValue + 0.5f * distanceStep : foundValue;

        return RNG.SeededRange((float)min, (float)max);
    }

    private float MidPoint {
        get {
            return max - (max - min) * 0.5f;
        }
    }
}