using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RefreshShop : MonoBehaviour
{
    public ShopController shopController;

    public LockShop lockShop;
    
    private Button button;
    private void Start()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(RefreshShopUnits);
    }

    private void RefreshShopUnits()
    {
        if (!lockShop.locked)
        {
            shopController.PopulateShopUnits();
        }
        else
        {
            return;
        }
    }
}
