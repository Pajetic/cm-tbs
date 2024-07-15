using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour {

    private const int ACTION_POINTS_MAX = 2;

    public static event EventHandler OnAnyActionPointsChanged;

    private GridPosition gridPosition;
    private MoveAction moveAction;
    private SpinAction spinAction;
    private BaseAction[] baseActionArray;
    private int actionPoints = ACTION_POINTS_MAX;

    private void Awake() {
        moveAction = GetComponent<MoveAction>();
        spinAction = GetComponent<SpinAction>();
        baseActionArray = GetComponents<BaseAction>();
    }

    private void Start() {
        gridPosition = LevelGrid.Instance.GetGridPosition(transform.position);
        LevelGrid.Instance.AddUnitAtGridPosition(gridPosition, this);
        TurnSystem.Instance.OnNextTurn += TurnSystem_OnNextTurn;
    }

    private void TurnSystem_OnNextTurn(object sender, System.EventArgs e) {
        actionPoints = ACTION_POINTS_MAX;
        OnAnyActionPointsChanged?.Invoke(sender, EventArgs.Empty);
    }

    private void Update() {
        GridPosition newGridPosition = LevelGrid.Instance.GetGridPosition(transform.position);
        if (newGridPosition != gridPosition) {
            LevelGrid.Instance.UnitMovedGridPosition(this, gridPosition, newGridPosition);
            gridPosition = newGridPosition;
        }
    }

    public MoveAction GetMoveAction() {
        return moveAction;
    }

    public GridPosition GetGridPosition() {
        return gridPosition;
    }

    public SpinAction GetSpinAction() {
        return spinAction;
    }

    public BaseAction[] GetBaseActionArray() {
        return baseActionArray;
    }

    public int GetActionPoints() {
        return actionPoints;
    }

    private bool CanAffordAction(BaseAction baseAction) {
        return actionPoints >= baseAction.GetActionPointCost();
    }

    public bool TrySpendActionPoints(BaseAction baseAction) {
        if (CanAffordAction(baseAction)) {
            SpendActionPoints(baseAction.GetActionPointCost());
            return true;
        }
        return false;
    }

    private void SpendActionPoints(int amount) {
        actionPoints -= amount;
        OnAnyActionPointsChanged?.Invoke(this, EventArgs.Empty);
    }

    private void OnDestroy() {
        TurnSystem.Instance.OnNextTurn -= TurnSystem_OnNextTurn;
    }
}
