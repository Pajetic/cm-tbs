using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthSystem : MonoBehaviour {

    public event EventHandler OnDeath;

    [SerializeField]private int health = 100;

    public void TakeDamage(int damageAmount) {
        health -= damageAmount;
        if (health < 0) {
            health = 0;
        }

        if (health == 0) {
            Die();
        }

        Debug.Log(health);
    }

    private void Die() {
        OnDeath?.Invoke(this, new EventArgs());
    }
}
