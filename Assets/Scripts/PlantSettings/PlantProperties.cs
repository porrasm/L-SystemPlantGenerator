﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class PlantProperties : ISetting {

    #region fields
    [SerializeField]
    private float targetLength, targetWidth;

    [SerializeField]
    private bool scaleToLength, scaleToWidth;

    [SerializeField]
    private float defaultAngle, startingOrientation, startingLineLength, startingLineWidth;

    [SerializeField]
    private Color startingColor;

    [SerializeField]
    private ProbabilityDistributionEditor angleVariance, startOrientationVariance, lineLengthVariance, lineWidthVariance, plantLengthVariance;

    public float TargetLength { get => targetLength; set => targetLength = value; }
    public float TargetWidth { get => targetWidth; set => targetWidth = value; }
    public bool ScaleToLength { get => scaleToLength; set => scaleToLength = value; }
    public bool ScaleToWidth { get => scaleToWidth; set => scaleToWidth = value; }
    public ProbabilityDistributionEditor AngleVariance { get => angleVariance; set => angleVariance = value; }
    public ProbabilityDistributionEditor StartOrientationVariance { get => startOrientationVariance; set => startOrientationVariance = value; }
    public ProbabilityDistributionEditor LineLengthVariance { get => lineLengthVariance; set => lineLengthVariance = value; }
    public ProbabilityDistributionEditor LineWidthVariance { get => lineWidthVariance; set => lineWidthVariance = value; }
    public ProbabilityDistributionEditor PlantLengthVariance { get => plantLengthVariance; set => plantLengthVariance = value; }
    public float DefaultAngle { get => defaultAngle; set => defaultAngle = value; }
    public float StartingOrientation { get => startingOrientation; set => startingOrientation = value; }
    public float StartingLineLength { get => startingLineLength; set => startingLineLength = value; }
    public float StartingLineWidth { get => startingLineWidth; set => startingLineWidth = value; }
    public Color StartingColor { get => startingColor; set => startingColor = value; }
    #endregion

    public PlantProperties() {
        targetLength = 0.5f;
        targetWidth = 0.5f;
        scaleToLength = true;

        defaultAngle = 15;
        startingOrientation = 0;
        startingLineLength = 0.25f;
        startingLineWidth = 0.05f;

        angleVariance = new ProbabilityDistributionEditor(100, -5, 5);
        startOrientationVariance = new ProbabilityDistributionEditor(100, -5, 5);
        lineLengthVariance = new ProbabilityDistributionEditor(100, -0.05f, 0.05f);
        lineWidthVariance = new ProbabilityDistributionEditor(100, -0.01f, 0.01f);
        plantLengthVariance = new ProbabilityDistributionEditor(100, -0.15f, 0.15f);
    }

    public void Validate() {
        Validator.Between(targetLength, 0, float.MaxValue, "Target length");
        Validator.Between(targetWidth, 0, float.MaxValue, "Target width");
        Validator.BetweenInclusive(defaultAngle, 0, 360, "Default angle");
        Validator.BetweenInclusive(StartingOrientation, 0, 360, "Orientation");
        Validator.Between(StartingLineLength, 0, float.MaxValue, "Line length");
        Validator.Between(startingLineWidth, 0, float.MaxValue, "Line width");
        Debug.Log("Validation not comeplet");
    }

    public LineState ToLineState() {
        LineState state = new LineState();
        state.Angle = DefaultAngle;
        state.Orientation = startingOrientation + startOrientationVariance.GetSeededFloat();
        state.Width = startingLineWidth;
        state.Length = startingLineLength;
        state.Color = startingColor;
        return state;
    }
}