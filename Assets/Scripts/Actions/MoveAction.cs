using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MoveAction : BaseAction {

    public event EventHandler OnStartMoving;
    public event EventHandler OnStopMoving;

    [SerializeField] private int maxMoveDistance = 4;
    private List<Vector3> positionList;
    private float moveSpeed = 4f;
    private float stoppingDistance = 0.1f;
    private float rotationSpeed = 20f;
    private int currentPositionIndex = 0;

    public void Update() {
        if (!isActive) {
            return;
        }

        Vector3 targetPosition = positionList[currentPositionIndex];
        Vector3 moveDirection = (targetPosition - transform.position).normalized;
        if (Vector3.Distance(transform.position, targetPosition) > stoppingDistance) {
            // Unit rotation
            //transform.forward = Vector3.Lerp(transform.forward, moveDirection, rotationSpeed * Time.deltaTime);
            transform.forward = Vector3.RotateTowards(transform.forward, moveDirection, rotationSpeed * Time.deltaTime, 0f);
            // Unit movement
            transform.position += moveDirection * moveSpeed * Time.deltaTime;
        } else {
            currentPositionIndex++;
            if (currentPositionIndex >= positionList.Count) {
                // Reached final destination
                OnStopMoving?.Invoke(this, EventArgs.Empty);
                ActionComplete();
            }
        }
    }

    public override void TakeAction(GridPosition gridPosition, Action onActionCompleteCallback) {
        List<GridPosition> pathGridPositionList = Pathfinding.Instance.FindPath(unit.GetGridPosition(), gridPosition, out int pathLength);
        currentPositionIndex = 0;
        positionList = new List<Vector3>();

        foreach (GridPosition pathGridPosition in pathGridPositionList) {
            positionList.Add(LevelGrid.Instance.GetWorldPosition(pathGridPosition));
        }
        ActionStart(onActionCompleteCallback);
        OnStartMoving?.Invoke(this, EventArgs.Empty);
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

                // Check for obstacles
                if (!Pathfinding.Instance.IsWalkableGridPosition(tryGridPosition)) {
                    continue;
                }

                // Check for unreachable tile
                if (!Pathfinding.Instance.HasPath(unitGridPosition, tryGridPosition)) {
                    continue;
                }

                // Check for path too lengthy
                int pathfindingDistanceMulti = 10;
                if (Pathfinding.Instance.GetPathLength(unitGridPosition, tryGridPosition) > maxMoveDistance * pathfindingDistanceMulti) {
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
