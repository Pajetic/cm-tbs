using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloorVisibility : MonoBehaviour {

    [SerializeField] private bool dynamicFloorPosition = false;
    [SerializeField] private List<Renderer> ignoreRendererList;
    private Renderer[] rendererArray;
    private int floor;
    private float floorVisibilityHeightOffset = 4f;

    private void Awake() {
        rendererArray = GetComponentsInChildren<Renderer>(true);
    }

    private void Start() {
        floor = LevelGrid.Instance.GetFloor(transform.position);

        if (!dynamicFloorPosition && floor == 0) {
            // Don't need this on first floor
            Destroy(this);
        }
    }

    private void Update() {
        if (dynamicFloorPosition) {
            floor = LevelGrid.Instance.GetFloor(transform.position);
        }
        if (floor == 0 || CameraController.Instance.GetCameraHeight() > LevelGrid.FLOOR_HEIGHT * floor + floorVisibilityHeightOffset) {
            Show();
        } else {
            Hide();
        }
    }

    private void Show() {
        foreach (Renderer renderer in rendererArray) {
            if (ignoreRendererList.Contains(renderer)) {
                continue;
            }
            renderer.enabled = true;
        }
    }

    private void Hide() {
        foreach (Renderer renderer in rendererArray) {
            if (ignoreRendererList.Contains(renderer)) {
                continue;
            }
            renderer.enabled = false;
        }
    }
}
