using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEditor.ProjectWindowCallback;
using UnityEngine;

public class Pathfinding : MonoBehaviour {

    public static Pathfinding Instance {  get; private set; }

    private const int MOVE_STRAIGHT_COST = 10;
    private const int MOVE_DIAGONAL_COST = 14;

    [SerializeField] private Transform gridDebugObjectPrefab;
    [SerializeField] private LayerMask obstaclesLayerMask;
    private int width;
    private int height;
    private float cellSize;
    private GridSystem<PathNode> gridSystem;

    private void Awake() {
        if (Instance != null) {
            Debug.LogError("Attempting to create multiple instances of Pathfinding! " + transform + " " + Instance);
            Destroy(gameObject);
            return;
        }
        Instance = this;
        
    }

    public void Setup(int width, int height, float cellSize) {
        this.width = width;
        this.height = height;
        this.cellSize = cellSize;
        gridSystem = new GridSystem<PathNode>(width, height, cellSize, (GridSystem<PathNode> g, GridPosition gridPosition) => new PathNode(gridPosition));
        //gridSystem.CreateDebugObjects(gridDebugObjectPrefab);
        float raycastOffsetDistance = 5f;
        for (int x = 0; x < width; x++) {
            for (int z = 0; z < height; z++) {
                GridPosition gridPosition = new GridPosition(x, z);
                if (Physics.Raycast(LevelGrid.Instance.GetWorldPosition(gridPosition) + Vector3.down * raycastOffsetDistance, Vector3.up, raycastOffsetDistance * 2, obstaclesLayerMask)) {
                    GetNode(x, z).SetIsWalkable(false);
                }
            }
        }
    }

    public List<GridPosition> FindPath(GridPosition startGridPosition, GridPosition endGridPosition, out int pathLength) {
        // Reset nodes
        for (int x = 0; x < gridSystem.GetWidth(); x++) {
            for (int z = 0; z < gridSystem.GetHeight(); z++) {
                PathNode pathNode = gridSystem.GetGridObject(new GridPosition(x, z));
                pathNode.SetGCost(int.MaxValue);
                pathNode.SetHCost(0);
                pathNode.CalculateFCost();
                pathNode.ResetCameFromPathNode();
            }
        }

        // Start pathfinding
        List<PathNode> openList = new List<PathNode>();
        List<PathNode> closedList = new List<PathNode>();

        PathNode startNode = gridSystem.GetGridObject(startGridPosition);
        PathNode endNode = gridSystem.GetGridObject(endGridPosition);
        openList.Add(startNode);

        startNode.SetGCost(0);
        startNode.SetHCost(CalculateDistance(startGridPosition, endGridPosition));
        while (openList.Count > 0) {
            PathNode currentNode = GetLowestFCostPathNode(openList);

            if (currentNode == endNode) {
                // Return the path
                pathLength = endNode.GetFCost();
                return CalculatePath(endNode);
            }

            openList.Remove(currentNode);
            closedList.Add(currentNode);

            foreach (PathNode neighbourNode in GetNeighbourList(currentNode)) {
                // Already checked
                if (closedList.Contains(neighbourNode)) {
                    continue;
                }

                // Can't walk
                if (!neighbourNode.IsWalkable()) {
                    closedList.Add(neighbourNode);
                    continue;
                }

                int tentativeGCost = currentNode.GetGCost() + CalculateDistance(currentNode.GetGridPosition(), neighbourNode.GetGridPosition());

                if (tentativeGCost < neighbourNode.GetGCost()) {
                    neighbourNode.SetCameFromPathNode(currentNode);
                    neighbourNode.SetGCost(tentativeGCost);
                    neighbourNode.SetHCost(CalculateDistance(neighbourNode.GetGridPosition(), endGridPosition));
                    neighbourNode.CalculateFCost();

                    if (!openList.Contains(neighbourNode)) {
                        openList.Add(neighbourNode);
                    }
                }
            }
        }

        // No path
        pathLength = 0;
        return null;
    }

    public int CalculateDistance(GridPosition gridPositionA, GridPosition gridPositionB) {
        int xDelta = Mathf.Abs(gridPositionA.x - gridPositionB.x);
        int zDelta = Mathf.Abs(gridPositionA.z - gridPositionB.z);
        return MOVE_DIAGONAL_COST * Mathf.Min(xDelta, zDelta) + MOVE_STRAIGHT_COST * Mathf.Abs(xDelta - zDelta);
    }

    private PathNode GetLowestFCostPathNode(List<PathNode> pathNodeList) {
        PathNode lowestFCostPathNode = pathNodeList[0];
        for (int i = 1; i < pathNodeList.Count; i++) {
            if (pathNodeList[i].GetFCost() < lowestFCostPathNode.GetFCost()) {
                lowestFCostPathNode = pathNodeList[i];
            }
        }
        return lowestFCostPathNode;
    }

    private PathNode GetNode(int x, int z) {
        return gridSystem.GetGridObject(new GridPosition(x, z));
    }

    private List<PathNode> GetNeighbourList(PathNode currentNode) {
        List<PathNode> neighbourList = new List<PathNode>();

        int[] cardinalNeighbourOffset = new int[] { 0, 0, 1, 1, 0 };
        GridPosition gridPosition = currentNode.GetGridPosition();

        if (gridPosition.x + 1 < gridSystem.GetWidth()) {
            neighbourList.Add(GetNode(gridPosition.x + 1, gridPosition.z + 0));
            if (gridPosition.z - 1 >= 0) {
                neighbourList.Add(GetNode(gridPosition.x + 1, gridPosition.z - 1));
            }

            if (gridPosition.z + 1 < gridSystem.GetHeight()) {
                neighbourList.Add(GetNode(gridPosition.x + 1, gridPosition.z + 1));
            }
        }
        
        if (gridPosition.x - 1 >= 0) {
            neighbourList.Add(GetNode(gridPosition.x - 1, gridPosition.z + 0));
            if (gridPosition.z - 1 >= 0) {
                neighbourList.Add(GetNode(gridPosition.x - 1, gridPosition.z - 1));
            }

            if (gridPosition.z + 1 < gridSystem.GetHeight()) {
                neighbourList.Add(GetNode(gridPosition.x - 1, gridPosition.z + 1));
            }
        }

        if (gridPosition.z - 1 >= 0) {
            neighbourList.Add(GetNode(gridPosition.x + 0, gridPosition.z - 1));
        }
        if (gridPosition.z + 1 < gridSystem.GetHeight()) {
            neighbourList.Add(GetNode(gridPosition.x + 0, gridPosition.z + 1));
        }
        
        return neighbourList;
    }

    private List<GridPosition> CalculatePath(PathNode endNode) {
        List<GridPosition> path = new List<GridPosition>();
        path.Add(endNode.GetGridPosition());
        PathNode currentNode = endNode;
        while (currentNode.GetCameFromPathNode() != null) {
            path.Add(currentNode.GetCameFromPathNode().GetGridPosition());
            currentNode = currentNode.GetCameFromPathNode();
        }
        path.Reverse();
        return path;
    }

    public bool IsWalkableGridPosition(GridPosition gridPosition) {
        return gridSystem.GetGridObject(gridPosition).IsWalkable();
    }

    public bool HasPath(GridPosition startGridPosition, GridPosition endGridPosition) {
        return FindPath(startGridPosition, endGridPosition, out int pathLength) != null;
    }

    public int GetPathLength(GridPosition startGridPosition, GridPosition endGridPosition) {
        FindPath(startGridPosition, endGridPosition, out int pathLength);
        return pathLength;
    }
}
