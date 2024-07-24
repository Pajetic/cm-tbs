using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEditor.ProjectWindowCallback;
using UnityEngine;

public class Pathfinding : MonoBehaviour {

    public static Pathfinding Instance {  get; private set; }

    private const int MOVE_STRAIGHT_COST = 10;
    private const int MOVE_DIAGONAL_COST = 14;

    [SerializeField] private Transform gridDebugObjectPrefab;
    [SerializeField] private LayerMask obstaclesLayerMask;
    [SerializeField] private LayerMask floorLayerMask;
    [SerializeField] private Transform pathfindingLinkContainer;
    private int width;
    private int height;
    private float cellSize;
    private int floorAmount;
    private List<GridSystem<PathNode>> gridSystemList;
    private List<PathfindingLink> pathfindingLinkList;

    private void Awake() {
        if (Instance != null) {
            Debug.LogError("Attempting to create multiple instances of Pathfinding! " + transform + " " + Instance);
            Destroy(gameObject);
            return;
        }
        Instance = this;
        
    }

    public void Setup(int width, int height, float cellSize, int floorAmount) {
        this.width = width;
        this.height = height;
        this.cellSize = cellSize;
        this.floorAmount = floorAmount;

        gridSystemList = new List<GridSystem<PathNode>>();
        for (int floor = 0; floor < floorAmount; floor++) {
            GridSystem<PathNode> gridSystem = new GridSystem<PathNode>(width, height, cellSize, floor, LevelGrid.FLOOR_HEIGHT, (GridSystem<PathNode> g, GridPosition gridPosition) => new PathNode(gridPosition));
            //gridSystem.CreateDebugObjects(gridDebugObjectPrefab);
            gridSystemList.Add(gridSystem);
        }
        float raycastOffsetDistance = 1f;
        for (int x = 0; x < width; x++) {
            for (int z = 0; z < height; z++) {
                // Mark floor walkable
                for (int floor = 0; floor < floorAmount; floor++) {
                    GridPosition gridPosition = new GridPosition(x, z, floor);
                    if (Physics.Raycast(LevelGrid.Instance.GetWorldPosition(gridPosition) + Vector3.up * raycastOffsetDistance, Vector3.down, raycastOffsetDistance * 2, floorLayerMask)) {
                        GetNode(x, z, floor).SetIsWalkable(true);
                    }
                } 

                // Mark obstacles unwalkable
                for (int floor = 0; floor < floorAmount; floor++) {
                    GridPosition gridPosition = new GridPosition(x, z, floor);
                    if (Physics.Raycast(LevelGrid.Instance.GetWorldPosition(gridPosition) + Vector3.down * raycastOffsetDistance, Vector3.up, raycastOffsetDistance * 2, obstaclesLayerMask)) {
                        GetNode(x, z, floor).SetIsWalkable(false);
                    }
                }
            }
        }

        pathfindingLinkList = new List<PathfindingLink>();
        foreach (Transform child in pathfindingLinkContainer) {
            if (child.TryGetComponent(out PathfindingLinkMono pathfindingLinkMono)) {
                pathfindingLinkList.Add(pathfindingLinkMono.GetPathfindingLink());
            }
        }
    }

    public List<GridPosition> FindPath(GridPosition startGridPosition, GridPosition endGridPosition, out int pathLength) {
        // Reset nodes
        for (int x = 0; x < width; x++) {
            for (int z = 0; z < height; z++) {
                for (int floor = 0; floor < floorAmount; floor++) {
                    PathNode pathNode = GetGridSystem(floor).GetGridObject(new GridPosition(x, z, floor));
                    pathNode.SetGCost(int.MaxValue);
                    pathNode.SetHCost(0);
                    pathNode.CalculateFCost();
                    pathNode.ResetCameFromPathNode();
                }
            }
        }

        // Start pathfinding
        List<PathNode> openList = new List<PathNode>();
        List<PathNode> closedList = new List<PathNode>();

        PathNode startNode = GetGridSystem(startGridPosition.floor).GetGridObject(startGridPosition);
        PathNode endNode = GetGridSystem(endGridPosition.floor).GetGridObject(endGridPosition);
        openList.Add(startNode);

        startNode.SetGCost(0);
        startNode.SetHCost(CalculateDistance(startGridPosition, endGridPosition));
        startNode.CalculateFCost();
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

    private GridSystem<PathNode> GetGridSystem(int floor) {
        return gridSystemList[floor];
    }

    private PathNode GetNode(int x, int z, int floor) {
        return GetGridSystem(floor).GetGridObject(new GridPosition(x, z, floor));
    }

    private List<PathNode> GetNeighbourList(PathNode currentNode) {
        List<PathNode> neighbourList = new List<PathNode>();

        GridPosition gridPosition = currentNode.GetGridPosition();

        if (gridPosition.x + 1 < width) {
            neighbourList.Add(GetNode(gridPosition.x + 1, gridPosition.z + 0, gridPosition.floor));
            if (gridPosition.z - 1 >= 0) {
                neighbourList.Add(GetNode(gridPosition.x + 1, gridPosition.z - 1, gridPosition.floor));
            }

            if (gridPosition.z + 1 < height) {
                neighbourList.Add(GetNode(gridPosition.x + 1, gridPosition.z + 1, gridPosition.floor));
            }
        }
        
        if (gridPosition.x - 1 >= 0) {
            neighbourList.Add(GetNode(gridPosition.x - 1, gridPosition.z + 0, gridPosition.floor));
            if (gridPosition.z - 1 >= 0) {
                neighbourList.Add(GetNode(gridPosition.x - 1, gridPosition.z - 1, gridPosition.floor));
            }

            if (gridPosition.z + 1 < height) {
                neighbourList.Add(GetNode(gridPosition.x - 1, gridPosition.z + 1, gridPosition.floor));
            }
        }

        if (gridPosition.z - 1 >= 0) {
            neighbourList.Add(GetNode(gridPosition.x + 0, gridPosition.z - 1, gridPosition.floor));
        }
        if (gridPosition.z + 1 < height) {
            neighbourList.Add(GetNode(gridPosition.x + 0, gridPosition.z + 1, gridPosition.floor));
        }
         
        neighbourList.AddRange(GetPathfindingLinkConnectedPathNodeList(gridPosition));
        
        return neighbourList;
    }

    private List<PathNode> GetPathfindingLinkConnectedPathNodeList(GridPosition gridPosition) {
        List<PathNode> gridPositionList = new List<PathNode>();

        foreach (PathfindingLink pathfindingLink in pathfindingLinkList) {
            if (pathfindingLink.gridPositionA == gridPosition) {
                gridPositionList.Add(GetNode(pathfindingLink.gridPositionB.x, pathfindingLink.gridPositionB.z, pathfindingLink.gridPositionB.floor));
            }
            if (pathfindingLink.gridPositionB == gridPosition) {
                gridPositionList.Add(GetNode(pathfindingLink.gridPositionA.x, pathfindingLink.gridPositionA.z, pathfindingLink.gridPositionA.floor));
            }
        }
        return gridPositionList;
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
        return GetGridSystem(gridPosition.floor).GetGridObject(gridPosition).IsWalkable();
    }

    public void SetIsWalkableGridPosition(GridPosition gridPosition, bool isWalkable) {
        GetGridSystem(gridPosition.floor).GetGridObject(gridPosition).SetIsWalkable(isWalkable);
    }

    public bool HasPath(GridPosition startGridPosition, GridPosition endGridPosition) {
        return FindPath(startGridPosition, endGridPosition, out int pathLength) != null;
    }

    public int GetPathLength(GridPosition startGridPosition, GridPosition endGridPosition) {
        FindPath(startGridPosition, endGridPosition, out int pathLength);
        return pathLength;
    }
}
