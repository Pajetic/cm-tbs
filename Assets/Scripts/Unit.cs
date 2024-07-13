using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour {

    [SerializeField] private Animator unitAnimator;

    private Vector3 targetPosition;
    private float moveSpeed = 4f;
    private float stoppingDistance = 0.1f;
    private float rotationSpeed = 20f;

    private void Awake() {
        targetPosition = transform.position;
    }

    private void Update() {
        if (Vector3.Distance(transform.position, targetPosition) > stoppingDistance) {
            // Move unit
            Vector3 moveDirection = (targetPosition - transform.position).normalized;
            transform.position += moveDirection * moveSpeed * Time.deltaTime;

            // Turn unit
            transform.forward = Vector3.Lerp(transform.forward, moveDirection, rotationSpeed * Time.deltaTime);

            // Run animation
            unitAnimator.SetBool("IsWalking", true);
        } else {
            // Idle animation
            unitAnimator.SetBool("IsWalking", false);
        }
    }

    public void Move(Vector3 targetPosition) {
        this.targetPosition = targetPosition;
    }

}
