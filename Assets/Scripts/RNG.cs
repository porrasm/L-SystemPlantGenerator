using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public static class RNG {

    private static System.Random nonSeededRandom;

    static RNG() {
        nonSeededRandom = new System.Random();
    }

    public static int Integer {
        get {
            return nonSeededRandom.Next();
        }
    }
    public static float Float {
        get {
            return (float)nonSeededRandom.NextDouble();
        }
    }

    public static float SeededFloat {
        get {
            return UnityEngine.Random.value;
        }
    }

    public static float SeededRange(float min, float max) {
        float diff = max - min;
        return min + RNG.SeededFloat * diff;
    }

    public static void SetSeed(int seed) {
        UnityEngine.Random.InitState(seed);
    }
}
