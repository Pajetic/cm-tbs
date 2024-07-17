using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class UnitRagdoll : MonoBehaviour {
    [SerializeField] Transform ragdollRootBone;

    public void Setup(Transform originalRootBone) {
        MatchAllChildTransforms(originalRootBone, ragdollRootBone);

        ApplyExplosionToRagdoll(ragdollRootBone, 300f, transform.position, 10f);
    }

    private void MatchAllChildTransforms(Transform root, Transform target) {
        foreach(Transform child in root) {
            Transform targetChild = target.Find(child.name);
            if (targetChild != null) {
                targetChild.position = child.position;
                targetChild.rotation = child.rotation;
                
                MatchAllChildTransforms(child, targetChild);
            }
        }
    }

    private void ApplyExplosionToRagdoll(Transform root, float explosionForce, Vector3 explosionPosition, float explosionRange) {
        foreach (Transform child in root) {
            if (child.TryGetComponent<Rigidbody>(out Rigidbody rigidbody)) {
                rigidbody.AddExplosionForce(explosionForce, explosionPosition, explosionRange);
                ApplyExplosionToRagdoll(child, explosionForce, explosionPosition, explosionRange);
            }
        }
    }
}
