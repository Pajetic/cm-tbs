using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TurnSystem : MonoBehaviour {

    public static TurnSystem Instance { get; private set; }

    public event EventHandler OnNextTurn;

    private int turnNumber = 1;

    private void Awake() {
        if (Instance != null) {
            Debug.LogError("Attempting to create multiple instances of TurnSystem! " + transform + " " + Instance);
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public void NextTurn() {
        turnNumber++;
        OnNextTurn?.Invoke(this, EventArgs.Empty);
    }

    public int GetTurnNumber() {
        return turnNumber;
    }
}
