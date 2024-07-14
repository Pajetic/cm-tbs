using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GridDebugObject : MonoBehaviour {

    [SerializeField] TextMeshPro gridDebugText;
    private GridObject gridObject;

    public void SetGridObject(GridObject gridObject) {
        this.gridObject = gridObject;
    }

    public void Update() {
        gridDebugText.text = gridObject.ToString();
    }
}
