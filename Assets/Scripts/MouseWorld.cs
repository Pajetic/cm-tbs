using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseWorld : MonoBehaviour {

    private static MouseWorld instance;

    [SerializeField] private LayerMask MousePlaneLayerMask;

    private void Awake() {
        instance = this;
    }

    public static Vector3 GetPosition() {
        Physics.Raycast(Camera.main.ScreenPointToRay(InputManager.Instance.GetMouseScreenPosition()), out RaycastHit raycastHit, float.MaxValue, instance.MousePlaneLayerMask);
        return raycastHit.point;
    }
}