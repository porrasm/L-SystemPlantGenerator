using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomPlantCreator {

    #region fields
    private PlantMeshGenerator generator;

    public bool Enabled { get; set; }

    private Transform customPlant;
    private Transform customPlantParts;

    #endregion

    public CustomPlantCreator(PlantMeshGenerator generator) {
        this.generator = generator;
    }

    public void Initialize() {
        InitTransforms();
    }
    private void InitTransforms() {
        customPlant = generator.transform.Find("Custom Plant");
        customPlantParts = generator.transform.Find("Custom Plant Parts");
        CleanTransforms();

        customPlant = new GameObject().transform;
        customPlant.parent = generator.transform;
        customPlant.name = "Custom Plant";

        customPlantParts = new GameObject().transform;
        customPlantParts.parent = generator.transform;
        customPlantParts.name = "Custom Plant Parts";
    }

    public void Clean() {
        CleanTransforms();
    }
    private void CleanTransforms() {
        if (customPlant != null) {
            MonoBehaviour.Destroy(customPlant.gameObject);
        }
        if (customPlantParts != null) {
            MonoBehaviour.Destroy(customPlantParts.gameObject);
        }
    }
}