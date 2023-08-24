using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LockShop : MonoBehaviour
{
    public GameObject openLock;
    public GameObject closedLock;

    public bool locked;
    
    private void Start()
    {
        Button button = GetComponent<Button>();
        button.onClick.AddListener(SetShopLocked);
    }

    private void SetShopLocked()
    {
        if (!locked)
        {
            locked = true;
            openLock.SetActive(false);
            closedLock.SetActive(true);
        }
        else
        {
            UnlockShop();
        }
    }

    private void UnlockShop()
    {
        locked = false;
        openLock.SetActive(true);
        closedLock.SetActive(false);
    }
}
