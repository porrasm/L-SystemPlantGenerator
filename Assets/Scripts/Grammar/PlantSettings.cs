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
    public string Axiom { get => axiom; }

    [SerializeField]
    private int iterations;
    public int Iterations { get => iterations; }

    [SerializeField]
    private bool useSeed;
    public bool UseSeed { get => useSeed; }

    [SerializeField]
    private int seed;
    public int Seed { get => seed; }

    [Header("Grammar settings")]
    [SerializeField]
    private LSystemGrammar grammar;
    public LSystemGrammar Grammar { get => grammar; }
    #endregion

    public void Validate() {
        grammar.Validate();

    }
}
