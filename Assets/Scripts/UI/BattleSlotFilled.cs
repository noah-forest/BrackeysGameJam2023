using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class BattleSlotFilled : MonoBehaviour, ISlotPayloadChangeHandler
{
	private Slot slot;

	private bool hasBeenInvoked;

	private GameObject xpBar;
	private XpBarController xpBarController;

	private void Awake()
	{
		slot = GetComponent<Slot>();
		xpBar = transform.GetChild(0).gameObject;
		xpBarController = xpBar.GetComponent<XpBarController>();
	}

	public void SlotPayloadChanged(GameObject payload)
	{
		Debug.Log("slot changed");
		if (payload == null)
		{
			xpBarController.ClearXpBar();
			HideXpBar();
		}
		else
		{
			xpBarController.ClearXpBar();
			ShowXpBar();
			xpBarController.InitXpBar();
		}
	}

	private void ShowXpBar()
	{
		xpBar.SetActive(true);
	}

	private void HideXpBar()
	{
		xpBar.SetActive(false);
	}
}
