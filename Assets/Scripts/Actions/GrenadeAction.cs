using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrenadeAction : BaseAction
{
    [SerializeField] private Transform grenadeProjectilePrefab;
    private int maxThrowDistance = 7;

    private void Update() {
        if (!isActive) {
            return;
        }
    }

    public override string GetActionName() {
        return "Grenade";
    }

    public override EnemyAIAction GetEnemyAIAction(GridPosition gridPosition) {
        return new EnemyAIAction {
            gridPosition = gridPosition,
            actionValue = 0,
        };
    }

    public override List<GridPosition> GetValidActionGridPositionList() {
        List<GridPosition> validGridPositionList = new List<GridPosition>();
        GridPosition unitGridPosition = unit.GetGridPosition();

        for (int x = -maxThrowDistance; x <= maxThrowDistance; x++) {
            for (int z = -maxThrowDistance; z <= maxThrowDistance; z++) {
                GridPosition tryGridPosition = new GridPosition(x, z, 0) + unitGridPosition;

                // Position out of bounds
                if (!LevelGrid.Instance.IsValidGridPosition(tryGridPosition)) {
                    continue;
                }

                // Restrict distance on diagonals
                if (Math.Abs(x) + Math.Abs(z) > maxThrowDistance) {
                    continue;
                }

                validGridPositionList.Add(tryGridPosition);
            }
        }

        return validGridPositionList;
    }

    public override void TakeAction(GridPosition targetPosition, Action onActionCompleteCallback) {
        Transform grenadeProjectileTransform = Instantiate(grenadeProjectilePrefab, unit.GetWorldPosition(), Quaternion.identity);
        grenadeProjectileTransform.GetComponent<GrenadeProjectile>().Setup(targetPosition, OnGrenadeBehaviorComplete);
        ActionStart(onActionCompleteCallback);
    }

    private void OnGrenadeBehaviorComplete() {
        ActionComplete();
    }
}
