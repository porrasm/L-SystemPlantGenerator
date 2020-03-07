using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomPlantCreator {

    #region fields
    private PlantMeshGenerator owner;

    public CustomPlantMeshGenerator customPlantGenerator { get; private set; }
    public CustomPlantSlaveGenerator customPlantPartsGenerator { get; private set; }
    public CustomPlantMeshGenerator selectedPartGenerator { get; private set; }

    public bool Enabled { get; set; }

    private Transform customTransform;
    private Transform customPlant;
    private Transform customPlantParts;
    private Transform selectedPart;

    #endregion

    public CustomPlantCreator(PlantMeshGenerator owner) {
        this.owner = owner;
    }

    #region initialization and cleanup
    public void Initialize() {
        InitTransforms();
        InitializeGenerators();
    }
    private void InitTransforms() {
        owner.CleanTransform();

        customTransform = new GameObject().transform;
        customTransform.parent = owner.transform;
        customTransform.name = "Custom Plant Creator";

        customPlant = new GameObject().transform;
        customPlant.parent = customTransform;
        customPlant.name = "Custom Plant";

        customPlantParts = new GameObject().transform;
        customPlantParts.parent = customTransform;
        customPlantParts.name = "Custom Plant Parts";

        selectedPart = new GameObject().transform;
        selectedPart.parent = customTransform;
        selectedPart.name = "Selected part";
    }
    private void InitializeGenerators() {
        customPlantGenerator = CustomPlantMeshGenerator.Add(customPlant.gameObject, owner);
        Logger.Print("Add slave");
        customPlantPartsGenerator = CustomPlantSlaveGenerator.Add(customPlantParts.gameObject, owner);
        selectedPartGenerator = CustomPlantMeshGenerator.Add(selectedPart.gameObject, owner);

        customPlantPartsGenerator.GeneratePlant();
        customPlantGenerator.Plant = customPlantPartsGenerator.Plant.Cut(0);
        customPlantGenerator.GeneratePlant();
    }

    public void Clean() {
        owner.CleanTransform();
    }
    #endregion

    public void Update() {
        UpdatePositions();
    }

    private void UpdatePositions() {

        Vector3 offset = owner.transform.position;

        customPlantParts.transform.position = offset + new Vector3(customPlantGenerator.PlantMesh.ScaledBounds.extents.x * 0.35f + 0f * customPlantPartsGenerator.PlantMesh.ScaledBounds.extents.x, 0);
        selectedPart.transform.position = CustomPlantEditor.ToWorldPos(Event.current.mousePosition);
    }
}