using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;
using UnityEngine.EventSystems;

public class UnitActionSystem : MonoBehaviour {

    public static UnitActionSystem Instance { get; private set; }

    public event EventHandler OnSelectedUnitChanged;
    public event EventHandler OnSelectedActionChanged;
    public event EventHandler<bool> OnBusyChanged;
    public event EventHandler OnActionStarted;

    [SerializeField] private Unit selectedUnit;
    [SerializeField] private LayerMask unitLayerMask;

    private BaseAction selectedAction;
    private bool isBusy = false;

    private void Awake() {
        if (Instance != null) {
            Debug.LogError("Attempting to create multiple instances of UnitActionSystem! " + transform + " " + Instance);
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start() {
        SetSelectedUnit(selectedUnit);
    }

    private void Update() {
        if (!TurnSystem.Instance.IsPlayerTurn()) {
            return;
        }

        if (isBusy) {
            return;
        }

        if (EventSystem.current.IsPointerOverGameObject()) {
            return;
        }

        if (TryHandleUnitSelection()) {
            return;
        }

        HandleAction();
    }

    private void HandleAction() {
        if (Input.GetMouseButtonDown(0)) {
            // Check invalid target location
            GridPosition mouseGridPosition = LevelGrid.Instance.GetGridPosition(MouseWorld.GetPosition());
            if (!selectedAction.IsValidActionGridPosition(mouseGridPosition)) {
                return;
            }

            // Check action point cost
            if (!selectedUnit.TrySpendActionPoints(selectedAction)) {
                return;
            }

            // Do action
            SetBusy();
            selectedAction.TakeAction(mouseGridPosition, ClearBusy);
        }
    }

    private bool TryHandleUnitSelection() {
        if (Input.GetMouseButtonDown(0)) {
            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out RaycastHit raycastHit, float.MaxValue, unitLayerMask)) {
                if (raycastHit.transform.TryGetComponent<Unit>(out Unit unit)) {
                    // Already selected
                    if (unit == selectedUnit) {
                        return false;
                    }
                    
                    // Is Enemy
                    if (unit.IsEnemy()) {
                        return false;
                    }
                    SetSelectedUnit(unit);
                    return true;
                }
            }
        }
        return false;
    }

    private void SetSelectedUnit(Unit unit) {
        selectedUnit = unit;
        SetSelectedAction(unit.GetMoveAction());
        OnSelectedUnitChanged?.Invoke(this, EventArgs.Empty);
    }

    public Unit GetSelectedUnit() {
        return selectedUnit;
    }

    public void SetSelectedAction(BaseAction baseAction) {
        selectedAction = baseAction;
        OnSelectedActionChanged?.Invoke(this, EventArgs.Empty);
    }

    public BaseAction GetSelectedAction() {
        return selectedAction;
    }

    private void SetBusy() {
        isBusy = true;
        OnBusyChanged?.Invoke(this, isBusy);
        OnActionStarted?.Invoke(this, EventArgs.Empty);
    }

    private void ClearBusy() {
        isBusy = false;
        OnBusyChanged?.Invoke(this, isBusy);
    }
}
