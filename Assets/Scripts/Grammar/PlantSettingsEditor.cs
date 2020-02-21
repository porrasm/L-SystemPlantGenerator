using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(PlantMeshGenerator))]
public class PlantSettingsEditor : Editor {

    #region fields
    [SerializeField]
    private bool grammarFold;
    #endregion

    public override void OnInspectorGUI() {
        //base.OnInspectorGUI();

        GeneralSettingsUI();
        GeneralPlantSettingUI();
        Settings.Validate();

        Actions();
    }

    private void GeneralSettingsUI() {
        InspectorGUI.CreateArea("General");
        Generator.Angle = InspectorGUI.FloatSlider("Angle", Generator.Angle, 0, 360);
        Generator.Width = InspectorGUI.FloatField("Width", Generator.Width, 0, Mathf.Infinity);
        Generator.Length = InspectorGUI.FloatField("Length", Generator.Length, 0, Mathf.Infinity);
        Generator.IterationFactor = InspectorGUI.FloatField("Iteration factor", Generator.IterationFactor, 0, Mathf.Infinity);

        InspectorGUI.EndArea();
    }
    private void GeneralPlantSettingUI() {
        InspectorGUI.CreateArea("Plant settings");
        Settings.Axiom = InspectorGUI.TextArea("Axiom", Settings.Axiom, int.MaxValue, true);
        Settings.Iterations = InspectorGUI.IntegerField("Iterations", Settings.Iterations);
        Settings.UseSeed = InspectorGUI.BoolField("Use seed", Settings.UseSeed);

        if (Settings.UseSeed) {
            Settings.Seed = InspectorGUI.IntegerSlider("Iterations", Settings.Iterations, 0, int.MaxValue - 10);
        }

        InspectorGUI.EndArea();

        GrammarSettings();
    }



    #region grammar rules
    private bool detailedRuleEditMode;

    private void GrammarSettings() {
        if (InspectorGUI.CreateFoldedArea("Grammar settings", ref grammarFold)) {

            // InspectorGUI.CreateBox();
            GrammarRules();
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Add character definition")) {
                Settings.Grammar.CharacterDefinitions.Add(new CharacterDefinition());
            }
            if (GUILayout.Button("Clear definitions")) {
                Settings.Grammar.CharacterDefinitions.Clear();
            }

            //  GUILayout.EndVertical();
            GUILayout.EndHorizontal();
        }
    }

    private void GrammarRules() {
        for (int i = 0; i < Settings.Grammar.CharacterDefinitions.Count; i++) {
            RuleSetting(Settings.Grammar.CharacterDefinitions[i], i);
            GUILayout.Space(10);
        }
    }
    private void RuleSetting(CharacterDefinition rule, int index) {
        InspectorGUI.CreateBox();

        GUILayout.BeginHorizontal();
        string charString = InspectorGUI.TextField("Character", "" + rule.Character);
        rule.Type = (CharacterDefinition.CharacterType)EditorGUILayout.EnumPopup(rule.Type);
        GUILayout.EndHorizontal();
        
        if (charString.Length == 0) {
            charString = "" + default(char);
        }

        rule.Character = char.ToLower(charString[charString.Length - 1]);

        if (rule.Type == CharacterDefinition.CharacterType.Alias) {
            rule.Alias = InspectorGUI.TextArea("Alias", rule.Alias, int.MaxValue, true);
        } else if (rule.Type == CharacterDefinition.CharacterType.Variable) {

            if (rule.Character == default(char)) {
                if (GUILayout.Button("Remove character definition")) {
                    Settings.Grammar.CharacterDefinitions.RemoveAt(index);
                }
            } else {
                GUILayout.Space(10);

                GUILayout.BeginHorizontal();
                GUILayout.Label("Rule");
                GUILayout.Label("Probability");
                GUILayout.EndHorizontal();
                for (int i = 0; i < rule.Rules.Count; i++) {
                    ProbabilityRule probRule = rule.Rules[i];
                    RuleRow(rule, i);
                }

                GUILayout.BeginHorizontal();
                if (GUILayout.Button("Add rule")) {
                    rule.Rules.Add(rule.DefaultRule);
                }
                if (GUILayout.Button("Edit")) {
                    detailedRuleEditMode = !detailedRuleEditMode;
                }

                GUILayout.EndHorizontal();

                GUILayout.Space(10);
                if (GUILayout.Button("Remove character definition")) {
                    Settings.Grammar.CharacterDefinitions.RemoveAt(index);
                }
            }
        }

        
        GUILayout.EndVertical();
    }
    private void RuleRow(CharacterDefinition charDef, int index) {
        GUILayout.BeginHorizontal();
        //if (GUILayout.Button("Remove")) {
        //    charDef.Rules.RemoveAt(index);
        //}

        ProbabilityRule rule = charDef.Rules[index];

        string rString;
        if (detailedRuleEditMode) {
            GUILayout.ExpandWidth(false);
            rString = EditorGUILayout.TextArea(rule.Rule);

            if (GUILayout.Button("Remove")) {
                charDef.Rules.RemoveAt(index);
            }
            //rule.Probability = InspectorGUI.FloatField("", rule.Probability, 0, 1);
            GUILayout.ExpandWidth(true);
        } else {
            rString = EditorGUILayout.TextField(rule.Rule);
            rule.Probability = InspectorGUI.FloatSlider("", rule.Probability, 0, 1);
        }

        if (rString == null || rString.Length == 0) {
            rule.Rule = "";
        } else {
            rule.Rule = rString.ToLower();
        }

        GUILayout.EndHorizontal();
    }
    #endregion

    #region actions
    private void Actions() {
        InspectorGUI.CreateBox();

        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Generate plant")) {
            Generator.GeneratePlant();
        }
        if (GUILayout.Button("Generate seed")) {
            Settings.Seed = RNG.Integer;
        }
        GUILayout.EndHorizontal();

        InspectorGUI.EndArea();
    }
    #endregion

    private PlantMeshGenerator Generator {
        get {
            return (PlantMeshGenerator)target;
        }
    }
    private PlantSettings Settings {
        get {
            return Generator.Settings;
        }
    }
}