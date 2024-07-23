using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestructibleCrate : MonoBehaviour {

    public static event EventHandler OnAnyDestroyed;

    [SerializeField] private Transform crateDestoryedPrefab;

    public void Damage() {
        Transform crateDestroyedTransform = Instantiate(crateDestoryedPrefab, transform.position, transform.rotation);
        ApplyExplosionToChildren(crateDestroyedTransform, 150f, transform.position, 10f);
        Destroy(gameObject);
        OnAnyDestroyed?.Invoke(this, EventArgs.Empty);
    }

    private void ApplyExplosionToChildren(Transform root, float explosionForce, Vector3 explosionPosition, float explosionRange) {
        foreach (Transform child in root) {
            if (child.TryGetComponent<Rigidbody>(out Rigidbody rigidbody)) {
                rigidbody.AddExplosionForce(explosionForce, explosionPosition, explosionRange);
                ApplyExplosionToChildren(child, explosionForce, explosionPosition, explosionRange);
            }
        }
    }
}
