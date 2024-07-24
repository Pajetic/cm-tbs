using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathfindingLinkMono : MonoBehaviour {

    [SerializeField] public Vector3 linkPositionA;
    [SerializeField] public Vector3 linkPositionB;

    public PathfindingLink GetPathfindingLink() {
        return new PathfindingLink {
            gridPositionA = LevelGrid.Instance.GetGridPosition(linkPositionA),
            gridPositionB = LevelGrid.Instance.GetGridPosition(linkPositionB),
        };
    }
}
