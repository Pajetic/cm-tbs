using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class LevelGrid : MonoBehaviour {

    public static LevelGrid Instance { get; private set; }

    public event EventHandler OnAnyUnitMovedGridPosition;

    [SerializeField] private Transform gridDebugObjectPrefab;
    private GridSystem<GridObject> gridSystem;
    [SerializeField] private int width;
    [SerializeField] private int height;
    [SerializeField] private float cellSize;

    private void Awake() {
        if (Instance != null) {
            Debug.LogError("Attempting to create multiple instances of LevelGrid! " + transform + " " + Instance);
            Destroy(gameObject);
            return;
        }
        Instance = this;

        gridSystem = new GridSystem<GridObject>(width, height, cellSize, (GridSystem<GridObject> gridSystem, GridPosition gridPosition) => new GridObject(gridSystem, gridPosition));
        //gridSystem.CreateDebugObjects(gridDebugObjectPrefab);
    }

    private void Start() {
        Pathfinding.Instance.Setup(width, height, cellSize);
    }

    public void AddUnitAtGridPosition(GridPosition gridPosition, Unit unit) {
        gridSystem.GetGridObject(gridPosition).AddUnit(unit);
    }

    public List<Unit> GetUnitListAtGridPosition(GridPosition gridPosition) {
        return gridSystem.GetGridObject(gridPosition).GetUnitList();
    }

    public void RemoveUnitAtGridPosition(GridPosition gridPosition, Unit unit) {
        gridSystem.GetGridObject(gridPosition).RemoveUnit(unit);
    }

    public void UnitMovedGridPosition(Unit unit, GridPosition fromPosition, GridPosition toPosition) {
        RemoveUnitAtGridPosition(fromPosition, unit);
        AddUnitAtGridPosition(toPosition, unit);
        OnAnyUnitMovedGridPosition?.Invoke(this, EventArgs.Empty);
    }

    public GridPosition GetGridPosition(Vector3 worldPosition) {
        return gridSystem.GetGridPosition(worldPosition);
    }

    public Vector3 GetWorldPosition(GridPosition gridPosition) {
        return gridSystem.GetWorldPosition(gridPosition);
    }

    public bool IsValidGridPosition(GridPosition gridPosition) {
        return gridSystem.IsValidGridPosition(gridPosition);
    }

    public bool HasUnitAtGridPosition(GridPosition gridPosition) {
        return gridSystem.GetGridObject(gridPosition).HasAnyUnit();
    }

    public int GetWidth() {
        return gridSystem.GetWidth();
    }

    public int GetHeight() {
        return gridSystem.GetHeight();
    }

    public Unit GetUnitAtGridPosition(GridPosition gridPosition) {
        return gridSystem.GetGridObject(gridPosition).GetUnit();
    }
}

