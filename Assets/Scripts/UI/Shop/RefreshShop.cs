using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RefreshShop : MonoBehaviour
{
    public ShopController shopController;
	public LockShop lockShop;

	private GameManager gameManager;

	private Button button;

    private void Start()
    {
        gameManager = GameManager.singleton;
        
        button = GetComponent<Button>();
        button.onClick.AddListener(CheckIfRefresh);
		gameManager.goldChangedEvent.AddListener(CheckForGold);
    }

    private void OnEnable()
    {
		if (lockShop.locked) return;
		if (!shopController.firstRoll) return;

		shopController.ClearShopWindows();
		shopController.PopulateShopUnits();
    }

	private void CheckForGold()
	{
		if (gameManager.Gold <= 0)
		{
			button.interactable = false;
		}
		else
		{
			button.interactable = true;
		}
	}

	private void CheckIfRefresh()
    {
		gameManager.Gold -= shopController.refreshCost;
		RefreshUnits();
    }

	private void RefreshUnits()
	{
		foreach (GameObject window in shopController.shopItemPos)
		{
			Animator animator = window.GetComponent<Animator>();
			animator.SetTrigger("refreshed");
		}

		shopController.ClearShopWindows();
		shopController.PopulateShopUnits();
	}
}
