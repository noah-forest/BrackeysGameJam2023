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
        button.onClick.AddListener(RefreshShopUnits);
    }

    private void OnEnable()
    {
		if (lockShop.locked) return;
		if (!shopController.firstRoll) return;
		if(button) button.interactable = true;

		shopController.ClearShopWindows();
		shopController.PopulateShopUnits();
    }

    private void RefreshShopUnits()
    {
		gameManager.Gold -= shopController.refreshCost;

		foreach(GameObject window in shopController.shopItemPos)
		{
			Animator animator = window.GetComponent<Animator>();
			animator.SetTrigger("refreshed");
		}

		if (gameManager.Gold <= 0)
		{
			gameManager.Gold = 0;
			button.interactable = false;
		}

		shopController.ClearShopWindows();
		shopController.PopulateShopUnits();
    }
}
