using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class UnitAnimator : MonoBehaviour {
    [SerializeField] private Animator animator;

    // Shoot Animation
    [SerializeField] private Transform bulletProjectilePrefab;
    [SerializeField] private Transform bulletSpawnTransform;

    private void Start() {
        if (TryGetComponent<MoveAction>(out MoveAction moveAction)) {
            moveAction.OnStartMoving += MoveAction_OnStartMoving;
            moveAction.OnStopMoving += MoveAction_OnStopMoving;
        }

        if (TryGetComponent<ShootAction>(out ShootAction shootAction)) {
            shootAction.OnShoot += ShootAction_OnShoot;
        }
    }

    private void ShootAction_OnShoot(object sender, ShootAction.OnShootEventArgs e) {
        animator.SetTrigger("Shoot");
        BulletProjectile bullet = Instantiate(bulletProjectilePrefab, bulletSpawnTransform.position, Quaternion.identity).GetComponent<BulletProjectile>();
        Vector3 bulletTarget = e.targetUnit.GetWorldPosition();
        bulletTarget.y = bulletSpawnTransform.position.y;
        bullet.Setup(bulletTarget);
    }

    private void MoveAction_OnStopMoving(object sender, EventArgs e) {
        animator.SetBool("IsWalking", false);
    }

    private void MoveAction_OnStartMoving(object sender, EventArgs e) {
        animator.SetBool("IsWalking", true);
    }
}
