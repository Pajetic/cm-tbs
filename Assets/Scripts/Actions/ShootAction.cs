using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ShootAction : BaseAction {

    public event EventHandler<OnShootEventArgs> OnShoot;

    public class OnShootEventArgs : EventArgs {
        public Unit targetUnit;
        public Unit shootingUnit;
    }

    private enum State {
        Aiming,
        Shooting,
        Recovering
    }

    private int maxShootDistance = 7;
    private State state;
    private float stateTimer;
    private float aimingStateTime = 1f;
    private float shootingStateTime = 0.1f;
    private float shootingRecoveryTime = 0.5f;
    private Unit targetUnit;
    private bool canShootBullet;
    private float rotationSpeed = 20f;

    private void Update() {
        if (!isActive) {
            return;
        }

        stateTimer -= Time.deltaTime;

        switch (state) {
            case State.Aiming:
                transform.forward = Vector3.Lerp(transform.forward, (targetUnit.GetWorldPosition() - unit.GetWorldPosition()).normalized, rotationSpeed * Time.deltaTime);
                break;
            case State.Shooting:
                if (canShootBullet) {
                    Shoot();
                    canShootBullet = false;
                }
                break;
            case State.Recovering:
                break;
        }

        if (stateTimer <= 0f) {
            NextState();
        }
    }

    private void NextState() {
        switch (state) {
            case State.Aiming:
                state = State.Shooting;
                stateTimer = shootingStateTime;
                break;
            case State.Shooting:
                state = State.Recovering;
                stateTimer = shootingRecoveryTime;
                break;
            case State.Recovering:
                base.ActionComplete();
                break;
        }
    }

    public override string GetActionName() {
        return "Shoot";
    }

    public override List<GridPosition> GetValidActionGridPositionList() {
        List<GridPosition> validGridPositionList = new List<GridPosition>();
        GridPosition unitGridPosition = unit.GetGridPosition();

        for (int x = -maxShootDistance; x <= maxShootDistance; x++) {
            for (int z = -maxShootDistance; z <= maxShootDistance; z++) {
                GridPosition tryGridPosition = new GridPosition(x, z) + unitGridPosition;

                // Position out of bounds
                if (!LevelGrid.Instance.IsValidGridPosition(tryGridPosition)) {
                    continue;
                }

                // Restrict distance on diagonals
                if (Math.Abs(x) + Math.Abs(z) > maxShootDistance) {
                    continue;
                }

                // Position has no unit
                if (!LevelGrid.Instance.HasUnitAtGridPosition(tryGridPosition)) {
                    continue;
                }

                // Unit belong to same team
                Unit target = LevelGrid.Instance.GetUnitAtGridPosition(tryGridPosition);
                if (unit.IsEnemy() == target.IsEnemy()) {
                    continue;
                }

                validGridPositionList.Add(tryGridPosition);
            }
        }

        return validGridPositionList;
    }

    public override void TakeAction(GridPosition position, Action onActionCompleteCallback) {
        targetUnit = LevelGrid.Instance.GetUnitAtGridPosition(position);

        state = State.Aiming;
        stateTimer = aimingStateTime;
        canShootBullet = true;
        ActionStart(onActionCompleteCallback);
    }

    private void Shoot() {
        OnShoot?.Invoke(this, new OnShootEventArgs {
            targetUnit = targetUnit,
            shootingUnit = unit
        });
        targetUnit.Damage(40);
    }

    public Unit GetTargetUnit() {
        return targetUnit;
    }

    public int GetMaxShootDistance() {
        return maxShootDistance;
    }
}