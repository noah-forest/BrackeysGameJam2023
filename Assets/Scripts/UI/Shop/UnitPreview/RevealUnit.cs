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

	private void Start()
	{
		mouseUtils = MouseUtils.singleton;
		gameManager = GameManager.singleton;
		gameManager.goldChangedEvent.AddListener(CheckForCash);

		button = GetComponent<Button>();
		button.onClick.AddListener(CheckIfReveal);

		unitSlot = GetComponent<Slot>();
		trigger = GetComponent<SlotTooltipTrigger>();

		unitCostText.text = gameManager.revealCost.ToString();
	}

	private void CheckForCash()
	{
		if (gameManager.Cash > 0 && gameManager.Cash >= gameManager.revealCost && gameManager.Cash - gameManager.revealCost >= 0)
		{
			button.interactable = true;
			canReveal = true;
		}
		else if (gameManager.Cash <= 0 || gameManager.Cash < gameManager.revealCost)
		{
			button.interactable = false;
			canReveal = false;
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
		button.interactable = false;
		priceTag.SetActive(false);
		hidden = false;
	}

	public void HideUnit()
	{
		unitImage.sprite = hiddenSprite;
		unitSlot.payload = null;
		if(canReveal)
		{
			button.interactable = true;
		}
		priceTag.SetActive(true);
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
