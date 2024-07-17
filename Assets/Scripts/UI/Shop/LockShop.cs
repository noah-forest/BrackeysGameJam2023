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
	public GameObject refreshCost;

	public Image lockBorder;
	public Sprite lockBorderTexture;

	private Sprite defaultBorderTexture;

	public Button refreshButton;
	public RefreshShop refreshShop;

    public bool locked;

	private GameManager gameManager;
    
    private void Start()
    {
		gameManager = GameManager.singleton;

		defaultBorderTexture = lockBorder.sprite;

		refreshShop = refreshButton.gameObject.GetComponent<RefreshShop>();

        Button button = GetComponent<Button>();
        button.onClick.AddListener(SetShopLocked);

		gameManager.startGame.AddListener(UnlockShop);
    }

	private void OnEnable()
	{
		if (locked)
		{
			LockShopItems();
		}
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
		refreshCost.SetActive(false);
		closedLock.SetActive(true);
		refreshLock.SetActive(true);


		lockBorder.color = new Color32(183, 75, 72, 255);
		lockBorder.sprite = lockBorderTexture;

		gameManager.shopLocked.Invoke();
	}

    private void UnlockShop()
    {
        locked = false;
        openLock.SetActive(true);
		refreshCost.SetActive(true);
		closedLock.SetActive(false);
		refreshLock.SetActive(false);

		lockBorder.color = new Color32(163, 143, 99, 255);
		lockBorder.sprite = defaultBorderTexture;

		gameManager.shopUnlocked.Invoke();
	}
}
