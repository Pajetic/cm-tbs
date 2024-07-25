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

    public static Vector3 GetPositionIgnoreHidden() {
        RaycastHit[] raycastHitArray = Physics.RaycastAll(Camera.main.ScreenPointToRay(InputManager.Instance.GetMouseScreenPosition()), float.MaxValue, instance.MousePlaneLayerMask);
        Array.Sort(raycastHitArray, (RaycastHit raycastHitA, RaycastHit raycastHitB) => {
            return Mathf.RoundToInt(raycastHitA.distance - raycastHitB.distance);
        });

        foreach (RaycastHit raycastHit in raycastHitArray) {
            if (raycastHit.transform.TryGetComponent(out Renderer renderer)) {
                if (renderer.enabled) {
                    return raycastHit.point;
                }
            }
        }
        return Vector3.zero;
    }
}