using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour {

    private const int ACTION_POINTS_MAX = 2;

    public static event EventHandler OnAnyActionPointsChanged;

    [SerializeField] private bool isEnemy;
    private GridPosition gridPosition;
    private HealthSystem healthSystem;
    private MoveAction moveAction;
    private SpinAction spinAction;
    private BaseAction[] baseActionArray;
    private int actionPoints = ACTION_POINTS_MAX;

    private void Awake() {
        healthSystem = GetComponent<HealthSystem>();
        moveAction = GetComponent<MoveAction>();
        spinAction = GetComponent<SpinAction>();
        baseActionArray = GetComponents<BaseAction>();
    }

    private void Start() {
        gridPosition = LevelGrid.Instance.GetGridPosition(transform.position);
        LevelGrid.Instance.AddUnitAtGridPosition(gridPosition, this);
        TurnSystem.Instance.OnNextTurn += TurnSystem_OnNextTurn;
        healthSystem.OnDeath += HealthSystem_OnDeath;
    }

    private void HealthSystem_OnDeath(object sender, EventArgs e) {
        LevelGrid.Instance.RemoveUnitAtGridPosition(gridPosition, this);
        Destroy(gameObject);
    }

    private void TurnSystem_OnNextTurn(object sender, System.EventArgs e) {
        if ((IsEnemy() && !TurnSystem.Instance.IsPlayerTurn()) ||
            (!IsEnemy() && TurnSystem.Instance.IsPlayerTurn())) {
            actionPoints = ACTION_POINTS_MAX;
            OnAnyActionPointsChanged?.Invoke(sender, EventArgs.Empty);
        }
    }

    private void Update() {
        GridPosition newGridPosition = LevelGrid.Instance.GetGridPosition(transform.position);
        if (newGridPosition != gridPosition) {
            GridPosition previousGridPosition = gridPosition;
            gridPosition = newGridPosition;
            LevelGrid.Instance.UnitMovedGridPosition(this, previousGridPosition, newGridPosition);
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

    public bool IsEnemy() {
        return isEnemy;
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

    public void Damage(int damageAmount) {
        healthSystem.TakeDamage(damageAmount);
    }

    public Vector3 GetWorldPosition() {
        return transform.position;
    }

    private void OnDestroy() {
        TurnSystem.Instance.OnNextTurn -= TurnSystem_OnNextTurn;
    }
}
