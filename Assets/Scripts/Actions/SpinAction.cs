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
            ActionComplete();
        }
    }

    public override void TakeAction(GridPosition gridPosition, Action onActionCompleteCallback) {
        totalSpinAmount = 0;
        ActionStart(onActionCompleteCallback);
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