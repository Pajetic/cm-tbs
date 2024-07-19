using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UnitWorldUI : MonoBehaviour {
    [SerializeField] private TextMeshProUGUI actionPointsText;
    [SerializeField] private Unit unit;
    [SerializeField] private Image healthBarProgress;
    [SerializeField] private HealthSystem healthSystem;

    private void Start() {
        Unit.OnAnyActionPointsChanged += Unit_OnAnyActionPointsChanged;
        healthSystem.OnHealthChanged += HealthSystem_OnHealthChanged;
        UpdateActionPointsText();
    }

    private void HealthSystem_OnHealthChanged(object sender, System.EventArgs e) {
        UpdateHealthBar();
    }

    private void Unit_OnAnyActionPointsChanged(object sender, System.EventArgs e) {
        UpdateActionPointsText();
    }

    private void UpdateActionPointsText() {
        actionPointsText.text = unit.GetActionPoints().ToString();
    }

    private void UpdateHealthBar() {
        healthBarProgress.fillAmount = healthSystem.GetHealthNormalized();
    }
}
