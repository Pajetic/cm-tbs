using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TurnSystem : MonoBehaviour {

    public static TurnSystem Instance { get; private set; }

    public event EventHandler OnNextTurn;

    private int turnNumber = 1;
    private bool isPlayerTurn = true;

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
        isPlayerTurn = !isPlayerTurn;
        OnNextTurn?.Invoke(this, EventArgs.Empty);
    }

    public int GetTurnNumber() {
        return turnNumber;
    }

    public bool IsPlayerTurn() {
        return isPlayerTurn;
    }
}
