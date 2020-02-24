using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Validator  {
    public static void Between(float value, float min, float max, string error) {
        if (value < min) {
            throw new System.Exception("value < min: " + error);
        }
        if (value > max) {
            throw new System.Exception("value > max: " + error);
        }
    }
    public static void BetweenInclusive(float value, float min, float max, string error) {
        if (value <= min) {
            throw new System.Exception("value <= min: " + error);
        }
        if (value >= max) {
            throw new System.Exception("value >= max: " + error);
        }
    }
}