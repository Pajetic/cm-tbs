using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TurnSystemUI : MonoBehaviour {

    [SerializeField] private Button endTurnButton;
    [SerializeField] private TextMeshProUGUI turnNumberText;

    private void Start() {
        TurnSystem.Instance.OnNextTurn += TurnSystem_OnNextTurn;

        endTurnButton.onClick.AddListener(() => {
            TurnSystem.Instance.NextTurn();
        });

        UpdateTurnText();
    }

    private void TurnSystem_OnNextTurn(object sender, System.EventArgs e) {
        UpdateTurnText();
    }

    private void UpdateTurnText() {
        turnNumberText.text = "TURN " + TurnSystem.Instance.GetTurnNumber();
    }

    private void OnDestroy() {
        TurnSystem.Instance.OnNextTurn -= TurnSystem_OnNextTurn;
    }
}
