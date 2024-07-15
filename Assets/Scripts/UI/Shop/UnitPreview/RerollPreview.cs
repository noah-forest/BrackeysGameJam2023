using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RerollPreview : MonoBehaviour
{
	public UnitPreview unitPreview;
	public TextMeshProUGUI rerollCostText;
	public bool canReroll;

	private Button button;
	private GameManager gameManager;

    // Start is called before the first frame update
    void Start()
    {
		gameManager = GameManager.singleton;
		gameManager.goldChangedEvent.AddListener(CheckForCash);

		button = GetComponent<Button>();
		button.onClick.AddListener(CheckIfReroll);

		rerollCostText.text = unitPreview.rerollCost.ToString();

		canReroll = true;
    }

	private void CheckForCash()
	{
		if (gameManager.Cash <= 0 || gameManager.Cash < unitPreview.rerollCost)
		{
			button.interactable = false;
			canReroll = false;
		}
		else if (gameManager.Cash > 0 && gameManager.Cash >= unitPreview.rerollCost)
		{
			button.interactable = true;
			canReroll = true;
		}
	}

	private void CheckIfReroll()
	{
		gameManager.Cash -= unitPreview.rerollCost;
		unitPreview.RerollEnemyUnits();
	}
}
