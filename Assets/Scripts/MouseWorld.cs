using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseWorld : MonoBehaviour {

    private static MouseWorld instance;

    [SerializeField] private LayerMask MousePlaneLayerMask;

    private void Awake() {
        if (instance == null) {
            instance = this;
        } else {
            Destroy(instance);
        }
    }

    public static Vector3 GetPosition() {
        Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out RaycastHit mouseWorldHit, float.MaxValue, instance.MousePlaneLayerMask);
        return mouseWorldHit.point;
    }
}