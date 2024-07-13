using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitSelectedVisual : MonoBehaviour {
    [SerializeField] private Unit unit;

    private MeshRenderer meshRenderer;

    private void Awake() {
        meshRenderer = GetComponent<MeshRenderer>();
    }

    private void Start() {
        UnitActionSystem.Instance.OnSelectedUnitChanged += OnSelectedUnitChanged_OnSelectedUnitChanged;

        UpdateVisual();
    }

    private void OnSelectedUnitChanged_OnSelectedUnitChanged(object sender, EventArgs e) {
        UpdateVisual();
    }

    private void UpdateVisual() {
        meshRenderer.enabled = UnitActionSystem.Instance.GetSelectedUnit() == unit ? true : false;
    }
}
