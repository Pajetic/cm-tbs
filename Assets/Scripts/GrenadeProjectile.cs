using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrenadeProjectile : MonoBehaviour {

    public static event EventHandler OnAnyGrenadeExploded;

    [SerializeField] private Transform grenadeExplodeVfxPrefab;
    [SerializeField] private TrailRenderer trailRenderer;
    [SerializeField] private AnimationCurve arcYAnimationCurve;
    private Vector3 targetPosition;
    private Action onGrenadeBehaviorComplete;
    private float moveSpeed = 15f;
    private float damageRadius = 4f;
    private int damageAmount = 30;
    private float totalDistance;
    private Vector3 positionXZ;

    private void Update() {
        Vector3 moveDir = (targetPosition - positionXZ).normalized;
        positionXZ += moveDir * moveSpeed * Time.deltaTime;

        float distance = Vector3.Distance(positionXZ, targetPosition);
        float distanceNormalized =  1 - distance / totalDistance;

        float curveMaxHeight = totalDistance / 4f;
        float positionY = arcYAnimationCurve.Evaluate(distanceNormalized) * curveMaxHeight;

        transform.position = new Vector3(positionXZ.x, positionY, positionXZ.z);

        float stoppingDistance = 0.2f;
        if (Vector3.Distance(transform.position,targetPosition) < stoppingDistance) {
            
            Collider[] colliderArray = Physics.OverlapSphere(targetPosition, damageRadius);
            foreach (Collider collider in colliderArray) {
                if (collider.TryGetComponent<Unit>(out Unit targetUnit)) {
                    targetUnit.Damage(damageAmount);
                }

                if (collider.TryGetComponent<DestructibleCrate>(out DestructibleCrate targetDestructibleCrate)) {
                    targetDestructibleCrate.Damage();
                }
            }
            OnAnyGrenadeExploded?.Invoke(this, EventArgs.Empty);
            trailRenderer.transform.parent = null;
            Instantiate(grenadeExplodeVfxPrefab, targetPosition + Vector3.up * 1f, Quaternion.identity);
            Destroy(gameObject);
            onGrenadeBehaviorComplete();
        }
    }

    public void Setup(GridPosition targetGridPosition, Action onGrenadeBehaviorComplete) {
        targetPosition = LevelGrid.Instance.GetWorldPosition(targetGridPosition);
        this.onGrenadeBehaviorComplete = onGrenadeBehaviorComplete;

        positionXZ = transform.position;
        positionXZ.y = 0f;
        totalDistance = Vector3.Distance(transform.position, targetPosition);
    }

}
