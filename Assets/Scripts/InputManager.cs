#define USE_NEW_INPUT_SYSTEM
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour {

    public static InputManager Instance { get; private set; }

    private PlayerInputActions playerInputActions;

    private void Awake() {
        if (Instance != null) {
            Debug.LogError("Attempting to create multiple instances of InputManager! " + transform + " " + Instance);
            Destroy(gameObject);
            return;
        }
        Instance = this;

        playerInputActions = new PlayerInputActions();
        playerInputActions.Player.Enable();
    }

    public Vector2 GetMouseScreenPosition() {
#if USE_NEW_INPUT_SYSTEM
        return Mouse.current.position.ReadValue();
#else
        return Input.mousePosition;
#endif
    }

    public bool IsMouseButtonDownThisFrame() {
#if USE_NEW_INPUT_SYSTEM
        return playerInputActions.Player.LMB.WasPressedThisFrame();
#else
        return Input.GetMouseButtonDown(0);
#endif
    }

    public Vector2 GetCameraMoveVector() {
#if USE_NEW_INPUT_SYSTEM
        return playerInputActions.Player.CameraMovement.ReadValue<Vector2>();
#else
        // Camera movement
        Vector2 inputMoveDirection = new Vector2(0, 0);
        if (Input.GetKey(KeyCode.W)) {
            inputMoveDirection.y = 1f;
        }
        if (Input.GetKey(KeyCode.S)) {
            inputMoveDirection.y = -1f;
        }
        if (Input.GetKey(KeyCode.A)) {
            inputMoveDirection.x = -1f;
        }
        if (Input.GetKey(KeyCode.D)) {
            inputMoveDirection.x = 1f;
        }
        return inputMoveDirection;
#endif
    }

    public float GetCameraRotateAmount() {
#if USE_NEW_INPUT_SYSTEM
        return playerInputActions.Player.CameraRotate.ReadValue<float>();
#else
        if (Input.GetKey(KeyCode.Q)) {
            return 1f;
        }

        if (Input.GetKey(KeyCode.E)) {
            return -1f;
        }

        return 0f;
#endif
    }

    public float GetCameraZoomAmount() {
#if USE_NEW_INPUT_SYSTEM
        return playerInputActions.Player.CameraZoom.ReadValue<float>();
#else
        if (Input.mouseScrollDelta.y > 0) {
            return -1f;
        }
        if (Input.mouseScrollDelta.y < 0) {
            return 1f;
        }

        return 0f;
#endif
    }
}