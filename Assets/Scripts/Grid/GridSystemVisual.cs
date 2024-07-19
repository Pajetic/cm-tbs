using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridSystemVisual : MonoBehaviour {

    public static GridSystemVisual Instance { get; private set; }

    [Serializable]
    public struct GridVisualTypeMaterial {
        public GridVisualType GridVisualType;
        public Material Material;
    }

    public enum GridVisualType {
        White,
        Blue,
        Red,
        RedSoft,
        Yellow,
    }

    [SerializeField] private Transform gridSystemVisualSingle;
    [SerializeField] private List<GridVisualTypeMaterial> gridVisualTypeMaterialList;

    private GridSystemVisualSingle[,] gridSystemVisualSingleArray;

    private void Awake() {
        if (Instance != null) {
            Debug.LogError("Attempting to create multiple instances of GridSystemVisual! " + transform + " " + Instance);
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start() {
        gridSystemVisualSingleArray = new GridSystemVisualSingle[LevelGrid.Instance.GetWidth(), LevelGrid.Instance.GetHeight()];

        for (int x = 0; x < LevelGrid.Instance.GetWidth(); x++) {
            for (int z = 0; z < LevelGrid.Instance.GetHeight(); z++) {
                Transform gridSingleTransform = Instantiate(gridSystemVisualSingle, LevelGrid.Instance.GetWorldPosition(new GridPosition(x, z)), Quaternion.identity);
                gridSystemVisualSingleArray[x, z] = gridSingleTransform.GetComponent<GridSystemVisualSingle>();
            }
        }

        UpdateGridVisual();

        UnitActionSystem.Instance.OnSelectedActionChanged += UnitActionSystem_OnSelectedActionChanged;
        LevelGrid.Instance.OnAnyUnitMovedGridPosition += LevelGrid_OnAnyUnitMovedGridPosition;
    }

    private void LevelGrid_OnAnyUnitMovedGridPosition(object sender, EventArgs e) {
        UpdateGridVisual();
    }

    private void UnitActionSystem_OnSelectedActionChanged(object sender, EventArgs e) {
        UpdateGridVisual();
    }

    public void HideAllGridPosition() {
        for(int x = 0; x < LevelGrid.Instance.GetWidth(); x++) {
            for (int z = 0; z < LevelGrid.Instance.GetHeight(); z++) {
                gridSystemVisualSingleArray[x, z].Hide();
            }
        }
    }

    private void ShowGridPositionList(List<GridPosition> gridPositionList, GridVisualType gridVisualType) {
        foreach(GridPosition gridPosition in gridPositionList) {
            gridSystemVisualSingleArray[gridPosition.x, gridPosition.z].Show(GetGridVisualTypeMaterial(gridVisualType));
        }
    }

    private void ShowGridPositionRange(GridPosition gridPosition, int range, GridVisualType gridVisualType) {
        List<GridPosition> gridPositionList = new List<GridPosition>();
        for (int x = -range; x <= range; x++) {
            for (int z = -range; z <= range; z++) {
                GridPosition tryGridPosition = new GridPosition(x, z) + gridPosition;

                // Position out of bounds
                if (!LevelGrid.Instance.IsValidGridPosition(tryGridPosition)) {
                    continue;
                }

                // Reduce diagonal range
                if (Math.Abs(x) + Math.Abs(z) > range) {
                    continue;
                }

                gridPositionList.Add(tryGridPosition);
            }
        }

        ShowGridPositionList(gridPositionList, gridVisualType);
    }

    private void UpdateGridVisual() {
        HideAllGridPosition();

        BaseAction selectedAction = UnitActionSystem.Instance.GetSelectedAction();
        GridVisualType gridVisualType;

        switch (selectedAction) {
            default:
            case MoveAction moveAction:
                gridVisualType = GridVisualType.White;
                break;
            case SpinAction spinAction:
                gridVisualType = GridVisualType.Blue;
                break;
            case ShootAction shootAction:
                gridVisualType = GridVisualType.Red;
                ShowGridPositionRange(UnitActionSystem.Instance.GetSelectedUnit().GetGridPosition(), shootAction.GetMaxShootDistance(), GridVisualType.RedSoft);
                break;

        }
        ShowGridPositionList(selectedAction.GetValidActionGridPositionList(), gridVisualType);
    }

    private Material GetGridVisualTypeMaterial(GridVisualType gridVisualType) {
        foreach (GridVisualTypeMaterial material in gridVisualTypeMaterialList) {
            if (material.GridVisualType == gridVisualType) {
                return material.Material;
            }
        }

        Debug.LogError("Could not find GridVisualTypeMaterial for " + gridVisualType);
        return null;
    }
}
