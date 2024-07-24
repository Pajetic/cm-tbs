using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(PathfindingLinkMono))]
public class PathfindingLinkMonoEditor : Editor {

    private void OnSceneGUI() {
        PathfindingLinkMono pathfindingLinkMono = (PathfindingLinkMono)target;

        EditorGUI.BeginChangeCheck();
        Vector3 newLinkPositionA = Handles.PositionHandle(pathfindingLinkMono.linkPositionA, Quaternion.identity);
        Vector3 newLinkPositionB = Handles.PositionHandle(pathfindingLinkMono.linkPositionB, Quaternion.identity);
        if (EditorGUI.EndChangeCheck()) {
            Undo.RecordObject(pathfindingLinkMono, "Change Link Position");
            pathfindingLinkMono.linkPositionA = newLinkPositionA;
            pathfindingLinkMono.linkPositionB = newLinkPositionB;
        }
    }

}
