using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class RevealUnit : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerUpHandler
{
	public Image unitImage;

	public Sprite currentSprite;
	public Sprite hiddenSprite;

	public GameObject unit;

	public GameObject priceTag;
	public TextMeshProUGUI unitCostText;

	public bool hidden;

	private Button button;
	private Slot unitSlot;
	private SlotTooltipTrigger trigger;

	private MouseUtils mouseUtils;
	private GameManager gameManager;

	private bool hovering;
	private bool canReveal;
	private bool revealed;

	private void Start()
	{
		mouseUtils = MouseUtils.singleton;
		gameManager = GameManager.singleton;

		button = GetComponent<Button>();
		button.onClick.AddListener(CheckIfReveal);

		unitSlot = GetComponent<Slot>();
		trigger = GetComponent<SlotTooltipTrigger>();

		unitCostText.text = gameManager.revealCost.ToString();
	}

	public bool CheckForGold()
	{
		if (gameManager.Cash > 0 && gameManager.Cash >= gameManager.revealCost && gameManager.Cash - gameManager.revealCost >= 0)
		{
			return true;
		}
		else
		{	
			return false;
		}
	}

	private void Update()
	{
		if (CheckForGold() && canReveal)
		{
			button.interactable = true;
		}
		else
		{
			button.interactable = false;
		}
	}

	private void CheckIfReveal()
	{
		gameManager.unitRevealed?.Invoke();

		gameManager.Cash -= gameManager.revealCost;
		ShowUnit();
	}

	public void ShowUnit()
	{
		unitImage.sprite = currentSprite;
		unitSlot.payload = unit;
		if (hovering)
		{
			trigger.ShowTooltip(unitSlot);
		}
		priceTag.SetActive(false);
		hidden = false;
		canReveal = false;
	}

	public void HideUnit()
	{
		unitImage.sprite = hiddenSprite;
		unitSlot.payload = null;
		priceTag.SetActive(true);
		canReveal = true;
		hidden = true;
	}

	public void OnPointerEnter(PointerEventData eventData)
	{
		hovering = true;

		if (hidden)
		{
			mouseUtils.SetHoverCursor();
		} else
		{
			mouseUtils.SetTooltipCursor();
		}
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		hovering = false;

		trigger.HideTooltip();
		mouseUtils.SetToDefaultCursor();
	}

	public void OnPointerUp(PointerEventData eventData)
	{
		if (hovering && canReveal)
		{
			mouseUtils.SetTooltipCursor();
		}
		else if (!hovering && !canReveal)
		{
			mouseUtils.SetToDefaultCursor();
		}
	}
}
