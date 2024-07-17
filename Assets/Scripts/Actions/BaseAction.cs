using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseAction : MonoBehaviour {

    protected Unit unit;
    protected bool isActive;
    protected Action onActionCompleteCallback;

    protected virtual void Awake() {
        unit = GetComponent<Unit>();
    }

    public abstract string GetActionName();

    public abstract void TakeAction(GridPosition position, Action onActionCompleteCallback);

    public virtual bool IsValidActionGridPosition(GridPosition gridPosition) {
        return GetValidActionGridPositionList().Contains(gridPosition);
    }

    public abstract List<GridPosition> GetValidActionGridPositionList();

    public virtual int GetActionPointCost() {
        return 1;
    }

    protected void ActionStart(Action onActionCompleteCallback) {
        isActive = true;
        this.onActionCompleteCallback = onActionCompleteCallback;
    }

    protected void ActionComplete() {
        isActive = false;
        onActionCompleteCallback?.Invoke();
    }
}
