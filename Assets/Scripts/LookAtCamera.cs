using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtCamera : MonoBehaviour {

    [SerializeField] private bool invert;
    private Transform cameraTransform;

    private void Awake() {
        cameraTransform = Camera.main.transform;
    }

    private void LateUpdate() {
        if (invert) {
            Vector3 invertedDir = transform.position - cameraTransform.position;
            transform.LookAt(transform.position + invertedDir);
        } else {
            transform.LookAt(cameraTransform.position);
        }
    }
}
