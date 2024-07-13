using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitActionSystem : MonoBehaviour {

    public static UnitActionSystem Instance { get; private set; }

    public event EventHandler OnSelectedUnitChanged;

    [SerializeField] private Unit selectedUnit;
    [SerializeField] private LayerMask unitLayerMask;

    private void Awake() {
        if (Instance != null) {
            Debug.LogError("Attempting to create multiple instances of UnitActionSystem! " + transform + " " + Instance);
            Destroy(gameObject);
        }

        Instance = this;
    }

    private void Update() {
        if (Input.GetMouseButtonDown(0)) {
            if (!TryHandleUnitSelection()) {
                selectedUnit.Move(MouseWorld.GetPosition());
            }
        }
    }

    private bool TryHandleUnitSelection() {
        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out RaycastHit raycastHit, float.MaxValue, unitLayerMask)) {
            if (raycastHit.transform.TryGetComponent<Unit>(out Unit unit)) {
                SetSelectedUnit(unit);
                return true;
            }
        }
        return false;
    }

    private void SetSelectedUnit(Unit unit) {
        selectedUnit = unit;
        OnSelectedUnitChanged?.Invoke(this, EventArgs.Empty);
    }

    public Unit GetSelectedUnit() {
        return selectedUnit;
    }
}
