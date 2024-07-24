using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractAction : BaseAction {

    private int maxInteractDistance = 1;

    public override string GetActionName() {
        return "Interact";
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

        for (int x = -maxInteractDistance; x <= maxInteractDistance; x++) {
            for (int z = -maxInteractDistance; z <= maxInteractDistance; z++) {
                GridPosition tryGridPosition = new GridPosition(x, z, 0) + unitGridPosition;

                // Position out of bounds
                if (!LevelGrid.Instance.IsValidGridPosition(tryGridPosition)) {
                    continue;
                }

                // No door
                IInteractable interactable = LevelGrid.Instance.GetInteractableAtGridPosition(tryGridPosition);
                if (interactable == null) {
                    continue;
                }

                validGridPositionList.Add(tryGridPosition);
            }
        }

        return validGridPositionList;
    }

    public override void TakeAction(GridPosition targetPosition, Action onActionCompleteCallback) {
        IInteractable interactable = LevelGrid.Instance.GetInteractableAtGridPosition(targetPosition);
        interactable.Interact(OnInteractComplete);
        ActionStart(onActionCompleteCallback);
    }

    private void OnInteractComplete() {
        ActionComplete();
    }
}
