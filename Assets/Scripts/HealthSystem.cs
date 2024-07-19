using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthSystem : MonoBehaviour {

    public event EventHandler OnHealthChanged;
    public event EventHandler OnDeath;

    [SerializeField] private int health = 100;
    private int healthMax;

    private void Awake() {
       healthMax = health;
    }

    public void TakeDamage(int damageAmount) {
        health -= damageAmount;
        if (health < 0) {
            health = 0;
        }

        OnHealthChanged?.Invoke(this, EventArgs.Empty);

        if (health == 0) {
            Die();
        }
    }

    private void Die() {
        OnDeath?.Invoke(this, new EventArgs());
    }

    public float GetHealthNormalized() {
        return (float)health / healthMax;
    }
}
