using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LockShop : MonoBehaviour
{
    public GameObject openLock;
    public GameObject closedLock;
	public GameObject refreshLock;
	public Button refreshButton;

    public bool locked;

	private GameManager gameManager;
    
    private void Start()
    {
		gameManager = GameManager.singleton;
        Button button = GetComponent<Button>();
        button.onClick.AddListener(SetShopLocked);
    }

    private void SetShopLocked()
    {
		if (!locked) LockShopItems();
		else UnlockShop();
    }

	private void LockShopItems()
	{
		locked = true;
		openLock.SetActive(false);
		closedLock.SetActive(true);
		refreshLock.SetActive(true);
		refreshButton.interactable = false;
	}

    private void UnlockShop()
    {
        locked = false;
        openLock.SetActive(true);
        closedLock.SetActive(false);
		refreshLock.SetActive(false);
		if(gameManager.Gold > 0) refreshButton.interactable = true;
	}
}
