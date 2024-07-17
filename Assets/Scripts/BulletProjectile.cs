using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletProjectile : MonoBehaviour {

    [SerializeField] TrailRenderer trailRenderer;
    [SerializeField] Transform bulletHitVfxPrefab;

    private Vector3 targetPosition;
    private float moveSpeed = 200f;

    private void Update() {
        Vector3 moveDirection = (targetPosition - transform.position).normalized;
        
        float distDeltaBefore = Vector3.Distance(transform.position, targetPosition);
        transform.position += moveDirection * moveSpeed * Time.deltaTime;
        float distDeltaAfter = Vector3.Distance(transform.position, targetPosition);
        if (distDeltaBefore < distDeltaAfter) {
            transform.position = targetPosition;
            trailRenderer.transform.parent = null;
            Destroy(gameObject);
            Instantiate(bulletHitVfxPrefab, targetPosition, Quaternion.identity);
        }
    }

    public void Setup(Vector3 targetPosition) {
        this.targetPosition = targetPosition;
    }

}
