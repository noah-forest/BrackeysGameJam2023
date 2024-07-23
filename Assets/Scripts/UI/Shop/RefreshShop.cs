using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class RefreshShop : MonoBehaviour
{
	public ShopController shopController;
	public LockShop lockShop;

	public TextMeshProUGUI refreshCostText;

	public bool canRefresh;

	private GameManager gameManager;

	private Button button;

	private int originalRefreshCost;

	private void Start()
	{
		gameManager = GameManager.singleton;

		originalRefreshCost = shopController.refreshCost;
		refreshCostText.text = shopController.refreshCost.ToString();

		button = GetComponent<Button>();
		button.onClick.AddListener(CheckIfRefresh);

		gameManager.shopLocked.AddListener(LockShop);

		canRefresh = true;
	}

	private void OnEnable()
	{
		shopController.refreshCost = originalRefreshCost;
		refreshCostText.text = shopController.refreshCost.ToString();

		shopController.refreshCost = 1;

		if (!canRefresh) return;
		if (!shopController.firstRoll) return;

		shopController.ClearShopWindows();
		shopController.PopulateShopUnits();
	}

	public bool CheckForGold()
	{
		if (gameManager.Cash > 0 && gameManager.Cash >= shopController.refreshCost && lockShop.locked == false)
		{
			return true;
		} else
		{
			return false;
		}
	}

	private void Update()
	{
		if (CheckForGold())
		{
			button.interactable = true;
			canRefresh = true;
		} else
		{
			LockShop();
		}
	}

	private void CheckIfRefresh()
	{
		gameManager.shopRefreshed.Invoke();

		gameManager.Cash -= shopController.refreshCost;
		if(shopController.refreshCost < 5)
		{
			++shopController.refreshCost;
			refreshCostText.text = shopController.refreshCost.ToString();
		}
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

	private void LockShop()
	{
		button.interactable = false;
		canRefresh = false;
	}
}
