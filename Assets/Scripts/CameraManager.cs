using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour {

    [SerializeField] private GameObject actionCameraGameObject;
    private float shootActionCameraVerticalOffset = 1.7f;
    private float shootActionCameraHorizontalOffset = 0.5f;

    private void Start() {
        BaseAction.OnAnyActionStarted += BaseAction_OnAnyActionStarted;
        BaseAction.OnAnyActionCompleted += BaseAction_OnAnyActionCompleted;
        HideActionCamera();
    }

    private void BaseAction_OnAnyActionCompleted(object sender, EventArgs e) {
        switch(sender) {
            case ShootAction shootAction:
                HideActionCamera();
                break;
            }
        }

    private void BaseAction_OnAnyActionStarted(object sender, EventArgs e) {
        switch (sender) {
            case ShootAction shootAction:
                Unit shooterUnit = shootAction.GetUnit();
                Unit targetUnit = shootAction.GetTargetUnit();
                Debug.Log("shooter " + shooterUnit + " : target " + targetUnit);
                Vector3 targetDirNormalized = (targetUnit.GetWorldPosition() - shooterUnit.GetWorldPosition()).normalized;

                Vector3 cameraPosition = shooterUnit.GetWorldPosition()
                    + Vector3.up * shootActionCameraVerticalOffset
                    + Quaternion.Euler(0, 90, 0) * targetDirNormalized * shootActionCameraHorizontalOffset
                    + targetDirNormalized * -1f;

                actionCameraGameObject.transform.position = cameraPosition;
                actionCameraGameObject.transform.LookAt(targetUnit.GetWorldPosition() + Vector3.up * shootActionCameraVerticalOffset);
                
                ShowActionCamera();
                break;
        }
    }

    private void ShowActionCamera() {
        actionCameraGameObject.SetActive(true);
    }

    private void HideActionCamera() {
        actionCameraGameObject.SetActive(false);
    }
}
