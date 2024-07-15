using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpinAction : BaseAction {

    private float totalSpinAmount;

    private void Update() {
        if (!isActive) {
            return;
        }

        float spinAddAmount = 360f * Time.deltaTime;
        transform.eulerAngles += new Vector3(0, spinAddAmount, 0);
        totalSpinAmount += spinAddAmount;
        if (totalSpinAmount > 360f) {
            isActive = false;
            onActionCompleteCallback?.Invoke();
        }
    }

    public override void TakeAction(GridPosition gridPosition, Action onActionCompleteCallback) {
        this.onActionCompleteCallback = onActionCompleteCallback;
        isActive = true;
        totalSpinAmount = 0;
    }

    public override string GetActionName() {
        return "Spin";
    }

    public override List<GridPosition> GetValidActionGridPositionList() {
        return new List<GridPosition> { unit.GetGridPosition() };
    }

    public override int GetActionPointCost() {
        return 2;
    }
}