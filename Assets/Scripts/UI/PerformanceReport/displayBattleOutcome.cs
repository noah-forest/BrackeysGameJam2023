using System;
using TMPro;
using UnityEngine;

public class displayBattleOutcome : MonoBehaviour
{
    private BattleManager battleManager;
    public TextMeshProUGUI outcomeText;

    private void OnEnable()
    {
        battleManager = BattleManager.singleton;
        if (battleManager.battleOutcome == "")
        {
            outcomeText.text = "No Battle Yet";
        }
        else
        {
            outcomeText.SetText($"Battle {battleManager.battleOutcome}");
            outcomeText.color = battleManager.battleOutcome == "Won" ? new Color32(159, 255, 97, 255) : new Color32(255, 95, 95, 255);
        }
    }
}
