using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : MonoBehaviour {

    private float timer = 0f;

    private void Start() {
        TurnSystem.Instance.OnNextTurn += TurnSystem_OnNextTurn;
    }

    private void TurnSystem_OnNextTurn(object sender, System.EventArgs e) {
        timer = 2f;
    }

    private void Update() {
        if (TurnSystem.Instance.IsPlayerTurn()) {
            return;
        }

        timer -= Time.deltaTime;
        if (timer <= 0) {
            TurnSystem.Instance.NextTurn();
        }
    }
}
