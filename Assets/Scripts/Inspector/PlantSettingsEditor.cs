using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(PlantMeshGenerator)), CanEditMultipleObjects]
public class PlantSettingsEditor : Editor {

    #region fields
    private PlantMeshGenerator generator;

    private bool grammarFold;

    public HashSet<object> MenuFolds { get; set; } = new HashSet<object>();

    private bool varianceFold;

    private long lastGenerateTime;
    private static int cycleIndex;
    #endregion

    public override void OnInspectorGUI() {
        //base.OnInspectorGUI();

        GeneralSettingsUI();
        GeneralPlantSettingUI();
        PlantPropertiesMenu();
        GrammarSettings();
        Settings.Validate();

        Actions();

        if (Generator.GenerateOnSettingChange) {
            StartGenerationCycle();
        }
    }

    private async void StartGenerationCycle() {

        cycleIndex++;
        int index = cycleIndex;

        long startTime = Timer.Time;

        while (Generator.GenerateOnSettingChange && Timer.Passed(startTime) < 5000 && index == cycleIndex) {

            Debug.Log("Cycle");

            long sinceLast = Timer.Passed(lastGenerateTime);

            if (sinceLast > Generator.GenerateTimeLimit) {
                Generator.GeneratePlant();
                lastGenerateTime = Timer.Time;
            }
            
            int delay = 100;

            if (Generator.GenerateTimeLimit > delay) {
                delay = Generator.GenerateTimeLimit;
            }

            await Task.Delay(delay);
        }

        Debug.Log("Stop cycle " + index);
    }

    private void OnValidate() {

    }

    private void GeneralSettingsUI() {
        InspectorGUI.CreateArea("General");
        Generator.GenerateOnSettingChange = InspectorGUI.BoolField("Generate on setting change", Generator.GenerateOnSettingChange);

        if (Generator.GenerateOnSettingChange) {
            Generator.GenerateTimeLimit = InspectorGUI.IntegerField("Generate time step", Generator.GenerateTimeLimit, 0, int.MaxValue);
        }

        GUILayout.Space(10);
        InspectorGUI.RangedFloatEditor(this, "Test float", Settings.FloatTet);

        InspectorGUI.EndArea();
    }
    private void GeneralPlantSettingUI() {
        InspectorGUI.CreateArea("Plant settings");
        Settings.Axiom = InspectorGUI.TextArea("Axiom", Settings.Axiom, int.MaxValue, true);
        Settings.Iterations = InspectorGUI.IntegerField("Iterations", Settings.Iterations);
        Settings.UseSeed = InspectorGUI.BoolField("Use seed", Settings.UseSeed);

        if (Settings.UseSeed) {
            Settings.Seed = InspectorGUI.IntegerSlider("Seed", Settings.Seed, 0, int.MaxValue - 10);
        }

        InspectorGUI.EndArea();
    }

    #region properties
    private void PlantPropertiesMenu() {

        PlantProperties prop = Settings.Properties;

        InspectorGUI.CreateBox();
        GUILayout.Label("Plant properties");
        GUILayout.Space(10);

        // Scaling
        prop.ScaleToLength = InspectorGUI.BoolField("Scale to length", prop.ScaleToLength);
        prop.ScaleToWidth = InspectorGUI.BoolField("Scale to width", prop.ScaleToWidth);

        if (prop.ScaleToLength) {
            InspectorGUI.RangedFloatEditor(this, "Target length", prop.TargetLength, 0, float.MaxValue);
        }
        if (prop.ScaleToWidth) {
            InspectorGUI.RangedFloatEditor(this, "Target width", prop.TargetWidth, 0, float.MaxValue);
        }

        // General

        GUILayout.Space(10);

        prop.DefaultAngle = InspectorGUI.FloatSlider("Default angle", prop.DefaultAngle, 0, 360);
        prop.StartingOrientation = InspectorGUI.FloatSlider("Starting orientation", prop.StartingOrientation, 0, 360);
        prop.StartingLineLength = InspectorGUI.FloatField("Starting line length", prop.StartingLineLength, 0, float.MaxValue);
        prop.StartingLineWidth = InspectorGUI.FloatField("Starting line width", prop.StartingLineWidth, 0, float.MaxValue);
        prop.StartingColor = InspectorGUI.ColorField("Starting color", prop.StartingColor);

        // Variance

        GUILayout.Space(10);
        if (InspectorGUI.CreateFoldedArea("Variance settings", ref varianceFold)) {
            InspectorGUI.IndependentDistributinEditor("Angle", this, prop.AngleVariance);
            InspectorGUI.IndependentDistributinEditor("Orientation", this, prop.StartOrientationVariance);
            InspectorGUI.IndependentDistributinEditor("Line length", this, prop.LineLengthVariance);
            InspectorGUI.IndependentDistributinEditor("Line width", this, prop.LineWidthVariance);
            InspectorGUI.EndFoldedArea();
        }


        InspectorGUI.EndArea();
    }
    #endregion

    #region state editor
    private void EditState(LineState state) {
        InspectorGUI.CreateBox();

        state.Orientation = InspectorGUI.FloatSlider("Orientation", state.Orientation, 0, 360);
        state.Width = InspectorGUI.FloatField("Width", state.Width);
        state.Length = InspectorGUI.FloatField("Length", state.Length);
        state.Color = InspectorGUI.ColorField("Color", state.Color);

        InspectorGUI.EndArea();
    }
    #endregion

    #region grammar rules
    private int detailedRuleEditMode;
    private void ToggleEdit() {
        detailedRuleEditMode++;
        if (detailedRuleEditMode > 2) {
            detailedRuleEditMode = 0;
        }
    }

    private void GrammarSettings() {
        if (InspectorGUI.CreateFoldedArea("Grammar settings", ref grammarFold)) {

            // InspectorGUI.CreateBox();
            GrammarRules();
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Add character definition")) {
                Settings.Grammar.CharacterDefinitions.Add(new CommandDefinition());
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
    private void RuleSetting(CommandDefinition rule, int index) {
        InspectorGUI.CreateBox();

        GUILayout.BeginHorizontal();
        string charString = InspectorGUI.TextField("Command", "" + rule.Command);

        rule.Type = (CommandDefinition.CommandType)EditorGUILayout.EnumPopup(rule.Type);
        GUILayout.EndHorizontal();

        rule.Command = charString;

        if (rule.Type == CommandDefinition.CommandType.Alias) {
            rule.Alias = InspectorGUI.TextArea("", rule.Alias, int.MaxValue, true);
            GUILayout.Space(10);
            if (GUILayout.Button("Remove character definition")) {
                Settings.Grammar.CharacterDefinitions.RemoveAt(index);
            }
        } else if (rule.Type == CommandDefinition.CommandType.Variable) {

            if (rule.Command.Length == 0) {
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
                    ToggleEdit();
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
    private void RuleRow(CommandDefinition charDef, int index) {
        GUILayout.BeginHorizontal();
        //if (GUILayout.Button("Remove")) {
        //    charDef.Rules.RemoveAt(index);
        //}

        ProbabilityRule rule = charDef.Rules[index];

        string rString;
        if (detailedRuleEditMode == 2) {
            GUILayout.ExpandWidth(false);
            rString = EditorGUILayout.TextField(rule.Rule);

            if (GUILayout.Button("Remove")) {
                charDef.Rules.RemoveAt(index);
            }
            //rule.Probability = InspectorGUI.FloatField("", rule.Probability, 0, 1);
            GUILayout.ExpandWidth(true);
        } else if (detailedRuleEditMode == 0) {
            rString = EditorGUILayout.TextField(rule.Rule);
            rule.Probability = InspectorGUI.FloatSlider("", rule.Probability, 0, 1);
        } else {
            rString = EditorGUILayout.TextArea(rule.Rule);
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

        if (Generator.GenerateOnSettingChange) {
            GUI.enabled = false;
            GUILayout.Button("Generate plant");
            GUI.enabled = true;
        } else {
            if (GUILayout.Button("Generate plant")) {
                //Settings.Seed = RNG.Integer;
                Generator.GeneratePlant();
            }
        }

        if (GUILayout.Button("Generate seed")) {
            Settings.Seed = RNG.Integer;
        }

        GUILayout.EndHorizontal();

        if (GUILayout.Button("Regenerate all")) {
            foreach (PlantMeshGenerator g in GameObject.FindObjectsOfType<PlantMeshGenerator>()) {
                g.GeneratePlant();
            }
        }

        InspectorGUI.EndArea();
    }
    #endregion


    private PlantMeshGenerator Generator {
        get {
            if (generator == null) {
                generator = (PlantMeshGenerator)target;
            }
            return generator;
        }
    }
    private PlantSettings Settings {
        get {
            return Generator.Settings;
        }
    }
}