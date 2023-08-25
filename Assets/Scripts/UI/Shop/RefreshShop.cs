using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RefreshShop : MonoBehaviour
{
    public ShopController shopController;
    public GameManager gameManager;
    
    public LockShop lockShop;
    
    private Button button;
    private void Start()
    {
        gameManager = GameManager.singleton;
        
        button = GetComponent<Button>();
        button.onClick.AddListener(RefreshShopUnits);
    }

    private void OnEnable()
    {
        if (!shopController) return;
        if (!lockShop.locked)
        {
            shopController.PopulateShopUnits();
        }
    }

    private void RefreshShopUnits()
    {
        if (gameManager.Gold <= 0) return;
        if (!lockShop.locked)
        {
            gameManager.Gold--;
            shopController.PopulateShopUnits();
        }
        else
        {
            return;
        }
    }
}
