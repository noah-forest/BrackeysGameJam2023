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

	private bool hovering;

	private void Start()
	{
		mouseUtils = MouseUtils.singleton;
	}

	private void OnEnable()
	{
		button = GetComponent<Button>();
		unitSlot = GetComponent<Slot>();
		trigger = GetComponent<SlotTooltipTrigger>();

		button.onClick.AddListener(ShowUnit);
	}

	public void ShowUnit()
	{
		unitImage.sprite = currentSprite;
		unitSlot.payload = unit;
		if (hovering)
		{
			trigger.ShowTooltip(unitSlot);
			// subtract money
		}
		button.interactable = false;
		priceTag.SetActive(false);
		hidden = false;
	}

	public void HideUnit()
	{
		unitImage.sprite = hiddenSprite;
		unitSlot.payload = null;
		button.interactable = true;
		//unitCostText.text = 
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
		if (hovering)
		{
			mouseUtils.SetTooltipCursor();
		} else
		{
			mouseUtils.SetToDefaultCursor();
		}
	}
}
