using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;

public class CameraController : MonoBehaviour {

    private const float MIN_FOLLOW_Y_OFFSET = 2f;
    private const float MAX_FOLLOW_Y_OFFSET = 12f;

    [SerializeField] private CinemachineVirtualCamera cinemachineVirtualCamera;

    private float moveSpeed = 10f;
    private float rotationSpeed = 100f;
    private float zoomIncrement = 1f;
    private float zoomSpeed = 5f;

    private Vector3 targetFollowOffset;

    private CinemachineTransposer cinemachineTransposer;

    private void Start() {
        cinemachineTransposer = cinemachineVirtualCamera.GetCinemachineComponent<CinemachineTransposer>();
        targetFollowOffset = cinemachineTransposer.m_FollowOffset;
    }

    private void Update() {
        HandleMovement();
        HandleRotation();
        HandleZoom();
    }

    private void HandleMovement() {
        // Camera movement
        Vector3 inputMoveDirection = new Vector3(0, 0, 0);
        if (Input.GetKey(KeyCode.W)) {
            inputMoveDirection.z = 1f;
        }
        if (Input.GetKey(KeyCode.S)) {
            inputMoveDirection.z = -1f;
        }
        if (Input.GetKey(KeyCode.A)) {
            inputMoveDirection.x = -1f;
        }
        if (Input.GetKey(KeyCode.D)) {
            inputMoveDirection.x = 1f;
        }

        Vector3 moveVector = transform.forward * inputMoveDirection.z + transform.right * inputMoveDirection.x;
        transform.position += moveVector * moveSpeed * Time.deltaTime;
    }

    private void HandleRotation() {
        // Camera rotation
        Vector3 rotationVector = new Vector3(0, 0, 0);
        if (Input.GetKey(KeyCode.Q)) {
            rotationVector.y = 1f;
        }
        if (Input.GetKey(KeyCode.E)) {
            rotationVector.y = -1f;
        }
        transform.eulerAngles += rotationVector * rotationSpeed * Time.deltaTime;
    }

    private void HandleZoom() {
        // Camera zoom
        if (Input.mouseScrollDelta.y > 0) {
            targetFollowOffset.y -= zoomIncrement;
        }
        if (Input.mouseScrollDelta.y < 0) {
            targetFollowOffset.y += zoomIncrement;
        }
        targetFollowOffset.y = Mathf.Clamp(targetFollowOffset.y, MIN_FOLLOW_Y_OFFSET, MAX_FOLLOW_Y_OFFSET);
        cinemachineTransposer.m_FollowOffset = Vector3.Lerp(cinemachineTransposer.m_FollowOffset, targetFollowOffset, zoomSpeed * Time.deltaTime);
    }
}
