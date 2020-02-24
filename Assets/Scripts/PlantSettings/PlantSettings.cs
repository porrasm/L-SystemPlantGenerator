using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

[Serializable]
public class PlantSettings : ISetting {

    #region fields

    // move to grammar class?
    [SerializeField]
    private string axiom;
    [SerializeField]
    private int iterations;
    [SerializeField]
    private bool useSeed;
    [SerializeField]
    private int seed;

    [SerializeField]
    private PlantProperties properties;

    [SerializeField]
    private LSystemGrammar grammar;

    [SerializeField]
    private RangedFloat floatTest;

    public string Axiom { get => axiom; set => axiom = value; }
    public int Iterations { get => iterations; set => iterations = value; }
    public bool UseSeed { get => useSeed; set => useSeed = value; }
    public int Seed { get => seed; set => seed = value; }

    public PlantProperties Properties { get => properties; }
    public LSystemGrammar Grammar { get => grammar; set => grammar = value; }
    public RangedFloat FloatTet { get => floatTest; set => floatTest = value; }
    #endregion

    public PlantSettings() {
        seed = RNG.Integer;
        properties = new PlantProperties();
        grammar = new LSystemGrammar();
        floatTest = new RangedFloat();
    }

    public void Validate() {

        if (Axiom != null) {
            Axiom = Axiom.ToLower();
        }

        grammar.Validate();
    }
}
