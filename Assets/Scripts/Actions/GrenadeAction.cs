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
                GridPosition tryGridPosition = new GridPosition(x, z) + unitGridPosition;

                // Position out of bounds
                if (!LevelGrid.Instance.IsValidGridPosition(tryGridPosition)) {
                    continue;
                }

                // Restrict distance on diagonals
                if (Math.Abs(x) + Math.Abs(z) > maxThrowDistance) {
                    continue;
                }

                //// Position has no unit
                //if (!LevelGrid.Instance.HasUnitAtGridPosition(tryGridPosition)) {
                //    continue;
                //}

                //// Unit belong to same team
                //Unit target = LevelGrid.Instance.GetUnitAtGridPosition(tryGridPosition);
                //if (unit.IsEnemy() == target.IsEnemy()) {
                //    continue;
                //}

                //float unitShootHeight = 1.7f;
                //Vector3 unitWorldPosition = LevelGrid.Instance.GetWorldPosition(unitGridPosition);
                //Vector3 shootDir = (target.GetWorldPosition() - unitWorldPosition).normalized;
                //// Check line of sight
                //if (Physics.Raycast(
                //    unitWorldPosition + Vector3.up * unitShootHeight,
                //    shootDir,
                //    Vector3.Distance(unitWorldPosition, target.GetWorldPosition()),
                //    obstaclesLayerMask)) {
                //    continue;
                //}

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
