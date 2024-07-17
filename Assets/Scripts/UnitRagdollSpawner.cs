using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitRagdollSpawner : MonoBehaviour {

    [SerializeField] private Transform ragdollPrefab;
    [SerializeField] private Transform originalRootBone;
    private HealthSystem healthSystem;

    private void Awake() {
        healthSystem = GetComponent<HealthSystem>();
    }

    private void Start() {
        healthSystem.OnDeath += HealthSystem_OnDeath;
    }

    private void HealthSystem_OnDeath(object sender, EventArgs e) {
        UnitRagdoll unitRagdoll = Instantiate(ragdollPrefab, transform.position, transform.rotation).GetComponent<UnitRagdoll>();
        unitRagdoll.Setup(originalRootBone);
    }
}
