using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SlotTooltipTrigger : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
	private Slot unitSlot;

	private UnitStats stats;
	private float Health;
	private int level;
	private UnitTooltipSystem tooltipSystem;

	private string header;

	private UnitRarity rarity;

	private MouseUtils mouseUtils;

	private void Start()
	{
		unitSlot = GetComponent<Slot>();
		tooltipSystem = UnitTooltipSystem.instance;
		mouseUtils = MouseUtils.singleton;
	}

	
	public void ShowTooltip(Slot slot)
	{
		if (slot.payload == null) return;

		header = slot.payload.name.Replace("(Clone)", "").Trim();
		stats = slot.payload.GetComponent<UnitStats>();
		level = slot.payload.GetComponent<Experience>().curLevel;
		Health = slot.payload.GetComponent<Health>().maxHealth;
		rarity = stats.Rarity;

		// raw stats to show
		tooltipSystem.levelTxt.text = $"{level}";
		tooltipSystem.healthTxt.text = $"{Health}";
		tooltipSystem.dmgTxt.text = $"{(float)stats.damage}";

		if (Settings.SimplifiedStats)
		{
			if ((stats.attackSpeed * 10) > 40) tooltipSystem.atkSpdTxt.text = "Booty";
			else if ((stats.attackSpeed * 10) == 40) tooltipSystem.atkSpdTxt.text = "Slow";
			else if ((stats.attackSpeed * 10) == 20) tooltipSystem.atkSpdTxt.text = "Average";
			else if ((stats.attackSpeed * 10) < 10) tooltipSystem.atkSpdTxt.text = "Nuts";
			else if ((stats.attackSpeed * 10) < 20) tooltipSystem.atkSpdTxt.text = "Fast";
		}
		else
		{
			tooltipSystem.atkSpdTxt.text = $"{stats.attackSpeed * 10}";
		}

		tooltipSystem.digCountTxt.text = $"{(int)stats.digCount}";

		tooltipSystem.unitDesc.text = stats.description;
		tooltipSystem.rarityTxt.text = rarity.ToString();
		SetLabelRarity(tooltipSystem.rarityLabel);

		tooltipSystem.blockChanceTxt.text = $"{stats.blockChance * 100f}";
		tooltipSystem.critChanceTxt.text = $"{stats.critChance * 100f}";

		UnitTooltipSystem.Show(header);
	}

	public void HideTooltip()
	{
		UnitTooltipSystem.Hide();
	}

	public void OnPointerEnter(PointerEventData eventData)
	{
		if (unitSlot.payload == null) return;
		ShowTooltip(unitSlot);
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		HideTooltip();
	}

	private void SetLabelRarity(Image label)
	{
		label.color = rarity switch
		{
			UnitRarity.Common => (Color)new Color32(169, 169, 169, 255),
			UnitRarity.Rare => (Color)new Color32(128, 187, 245, 255),
			UnitRarity.Epic => (Color)new Color32(218, 128, 245, 255),
			UnitRarity.Legendary => (Color)new Color32(255, 175, 85, 255),
			_ => (Color)new Color32(169, 169, 169, 255),
		};
	}
}
