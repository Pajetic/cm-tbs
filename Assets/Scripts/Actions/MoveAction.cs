using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveAction : BaseAction {

    public event EventHandler OnStartMoving;
    public event EventHandler OnStopMoving;

    [SerializeField] private int maxMoveDistance = 4;
    private Vector3 targetPosition;
    private float moveSpeed = 4f;
    private float stoppingDistance = 0.1f;
    private float rotationSpeed = 20f;

    protected override void Awake() {
        base.Awake();
        targetPosition = transform.position;
    }

    public void Update() {
        if (!isActive) {
            return;
        }

        if (Vector3.Distance(transform.position, targetPosition) > stoppingDistance) {
            // Unit movement
            Vector3 moveDirection = (targetPosition - transform.position).normalized;
            transform.position += moveDirection * moveSpeed * Time.deltaTime;

            // Unit rotation
            transform.forward = Vector3.Lerp(transform.forward, moveDirection, rotationSpeed * Time.deltaTime);

            // Run animation
        } else {
            // Idle animation
            base.ActionComplete();
            OnStopMoving?.Invoke(this, EventArgs.Empty);
        }
    }

    public override void TakeAction(GridPosition gridPosition, Action onActionCompleteCallback) {
        targetPosition = LevelGrid.Instance.GetWorldPosition(gridPosition);
        OnStartMoving?.Invoke(this, EventArgs.Empty);
        ActionStart(onActionCompleteCallback);
    }

    public override List<GridPosition> GetValidActionGridPositionList() {
        List<GridPosition> validGridPositionList = new List<GridPosition>();
        GridPosition unitGridPosition = unit.GetGridPosition();

        for (int x = -maxMoveDistance; x <= maxMoveDistance; x++) {
            for (int z = -maxMoveDistance; z <= maxMoveDistance; z++) {
                GridPosition tryGridPosition = new GridPosition(x, z) + unitGridPosition;

                // Position out of bounds
                if (!LevelGrid.Instance.IsValidGridPosition(tryGridPosition)) {
                    continue;
                }

                // Position occupied by self
                if (unitGridPosition == tryGridPosition) {
                    continue;
                }

                // Position occupied by other unit
                if (LevelGrid.Instance.HasUnitAtGridPosition(tryGridPosition)) {
                    continue;
                }

                validGridPositionList.Add(tryGridPosition);
            }
        }

        return validGridPositionList;
    }

    public override string GetActionName() {
        return "Move";
    }

    public override EnemyAIAction GetEnemyAIAction(GridPosition gridPosition) {
        return new EnemyAIAction {
            gridPosition = gridPosition,
            actionValue = unit.GetAction<ShootAction>().GetTargetCountAtPosition(gridPosition) * 10,
        };
    }
}
