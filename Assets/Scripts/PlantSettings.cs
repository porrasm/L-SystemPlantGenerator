using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

[Serializable]
public class PlantSettings : ISetting {

    #region fields
    [SerializeField]
    private string axiom;
    [SerializeField]
    private int iterations;
    [SerializeField]
    private bool useSeed;
    [SerializeField]
    private int seed;

    [SerializeField]
    private LineState initialState;

    [SerializeField]
    private LSystemGrammar grammar;

    [SerializeField]
    private RangedFloat floatTest;

    public string Axiom { get => axiom; set => axiom = value; }
    public int Iterations { get => iterations; set => iterations = value; }
    public bool UseSeed { get => useSeed; set => useSeed = value; }
    public int Seed { get => seed; set => seed = value; }
    public LSystemGrammar Grammar { get => grammar; set => grammar = value; }
    public LineState InitialState { get => initialState; set => initialState = value; }
    public RangedFloat FloatTet { get => floatTest; set => floatTest = value; }
    #endregion

    public PlantSettings() {
        seed = RNG.Integer;
        grammar = new LSystemGrammar();
        initialState = new LineState();
        floatTest = new RangedFloat();
    }

    public void Validate() {

        if (Axiom != null) {
            Axiom = Axiom.ToLower();
        }

        grammar.Validate();
    }
}
