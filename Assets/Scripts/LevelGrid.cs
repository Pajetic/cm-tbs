using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class LevelGrid : MonoBehaviour {

    public static LevelGrid Instance { get; private set; }

    public const float FLOOR_HEIGHT = 3f;

    public event EventHandler OnAnyUnitMovedGridPosition;

    [SerializeField] private Transform gridDebugObjectPrefab;
    private List<GridSystem<GridObject>> gridSystemList;
    [SerializeField] private int width;
    [SerializeField] private int height;
    [SerializeField] private float cellSize;
    [SerializeField] private int floorAmount;

    private void Awake() {
        if (Instance != null) {
            Debug.LogError("Attempting to create multiple instances of LevelGrid! " + transform + " " + Instance);
            Destroy(gameObject);
            return;
        }
        Instance = this;

        gridSystemList = new List<GridSystem<GridObject>>();
        for (int floor = 0; floor < floorAmount; floor++) {
            GridSystem<GridObject> gridSystem = new GridSystem<GridObject>(width, height, cellSize, floor, FLOOR_HEIGHT, (GridSystem<GridObject> gridSystem, GridPosition gridPosition) => new GridObject(gridSystem, gridPosition));
            //gridSystem.CreateDebugObjects(gridDebugObjectPrefab);
            gridSystemList.Add(gridSystem);
        }

    }

    private void Start() {
        Pathfinding.Instance.Setup(width, height, cellSize, floorAmount);
    }

    private GridSystem<GridObject> GetGridSystem(int floor) {
        return gridSystemList[floor];
    }

    public void AddUnitAtGridPosition(GridPosition gridPosition, Unit unit) {
        GetGridSystem(gridPosition.floor).GetGridObject(gridPosition).AddUnit(unit);
    }

    public List<Unit> GetUnitListAtGridPosition(GridPosition gridPosition) {
        return GetGridSystem(gridPosition.floor).GetGridObject(gridPosition).GetUnitList();
    }

    public void RemoveUnitAtGridPosition(GridPosition gridPosition, Unit unit) {
        GetGridSystem(gridPosition.floor).GetGridObject(gridPosition).RemoveUnit(unit);
    }

    public void UnitMovedGridPosition(Unit unit, GridPosition fromPosition, GridPosition toPosition) {
        RemoveUnitAtGridPosition(fromPosition, unit);
        AddUnitAtGridPosition(toPosition, unit);
        OnAnyUnitMovedGridPosition?.Invoke(this, EventArgs.Empty);
    }

    public int GetFloor(Vector3 worldPosition) {
        return Mathf.RoundToInt(worldPosition.y / FLOOR_HEIGHT);
    }

    public GridPosition GetGridPosition(Vector3 worldPosition) {
        return GetGridSystem(GetFloor(worldPosition)).GetGridPosition(worldPosition);
    }

    public Vector3 GetWorldPosition(GridPosition gridPosition) {
        return GetGridSystem(gridPosition.floor).GetWorldPosition(gridPosition);
    }

    public bool IsValidGridPosition(GridPosition gridPosition) {
        if (gridPosition.floor < 0 || gridPosition.floor >= floorAmount) {
            return false;
        }
        return GetGridSystem(gridPosition.floor).IsValidGridPosition(gridPosition);
    }

    public int GetWidth() {
        return GetGridSystem(0).GetWidth();
    }

    public int GetHeight() {
        return GetGridSystem(0).GetHeight();
    }

    public int GetFloorAmount() {
        return floorAmount;
    }

    public bool HasUnitAtGridPosition(GridPosition gridPosition) {
        return GetGridSystem(gridPosition.floor).GetGridObject(gridPosition).HasAnyUnit();
    }

    public Unit GetUnitAtGridPosition(GridPosition gridPosition) {
        return GetGridSystem(gridPosition.floor).GetGridObject(gridPosition).GetUnit();
    }

    public IInteractable GetInteractableAtGridPosition(GridPosition gridPosition) {
        return GetGridSystem(gridPosition.floor).GetGridObject(gridPosition).GetInteractable();
    }

    public void SetInteractableAtGridPosition(GridPosition gridPosition, IInteractable interactable) {
        GetGridSystem(gridPosition.floor).GetGridObject(gridPosition).SetInteractable(interactable);
    }
}

