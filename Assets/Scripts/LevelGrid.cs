using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelGrid : MonoBehaviour {

    public static LevelGrid Instance { get; private set; }

    [SerializeField] private Transform gridDebugObjectPrefab;
    private GridSystem gridSystem;

    private void Awake() {
        if (Instance != null) {
            Debug.LogError("Attempting to create multiple instances of LevelGrid! " + transform + " " + Instance);
            Destroy(gameObject);
        }
        Instance = this;

        gridSystem = new GridSystem(10, 10, 2);
        gridSystem.CreateDebugObjects(gridDebugObjectPrefab);
    }

    public void AddUnitAtGridPosition(GridPosition gridPosition, Unit unit) {
        gridSystem.GetGridObject(gridPosition).AddUnit(unit);
    }

    public List<Unit> GetUnitAtGridPosition(GridPosition gridPosition) {
        return gridSystem.GetGridObject(gridPosition).GetUnitList();
    }

    public void RemoveUnitAtGridPosition(GridPosition gridPosition, Unit unit) {
        gridSystem.GetGridObject(gridPosition).RemoveUnit(unit);
    }

    public void UnitMovedGridPosition(Unit unit, GridPosition fromPosition, GridPosition toPosition) {
        RemoveUnitAtGridPosition(fromPosition, unit);
        AddUnitAtGridPosition(toPosition, unit);
    }

    public GridPosition GetGridPosition(Vector3 worldPosition) {
        return gridSystem.GetGridPosition(worldPosition);
    }
}

