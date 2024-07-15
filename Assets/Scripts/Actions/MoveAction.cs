using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveAction : BaseAction {

    [SerializeField] private Animator unitAnimator;
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
            // Move unit
            Vector3 moveDirection = (targetPosition - transform.position).normalized;
            transform.position += moveDirection * moveSpeed * Time.deltaTime;

            // Turn unit
            transform.forward = Vector3.Lerp(transform.forward, moveDirection, rotationSpeed * Time.deltaTime);

            // Run animation
            unitAnimator.SetBool("IsWalking", true);
        } else {
            // Idle animation
            unitAnimator.SetBool("IsWalking", false);
            isActive = false;
            onActionCompleteCallback?.Invoke();
        }
    }

    public override void TakeAction(GridPosition gridPosition, Action onActionCompleteCallback) {
        this.onActionCompleteCallback = onActionCompleteCallback;
        targetPosition = LevelGrid.Instance.GetWorldPosition(gridPosition);
        isActive = true;
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
}
