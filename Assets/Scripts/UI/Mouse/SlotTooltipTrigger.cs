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
	private TooltipSystem tooltipSystem;

	private string header;

	private UnitRarity rarity;

	private void Start()
	{
		unitSlot = GetComponent<Slot>();
		tooltipSystem = TooltipSystem.instance;
	}

	public void OnPointerEnter(PointerEventData eventData)
	{
		if (unitSlot.payload == null) return;

		header = unitSlot.payload.name.Replace("(Clone)","").Trim();
		stats = unitSlot.payload.GetComponent<UnitStats>();
		level = unitSlot.payload.GetComponent<Experience>().curLevel;
		Health = unitSlot.payload.GetComponent<Health>().maxHealth;
		rarity = stats.Rarity;

		// raw stats to show
		tooltipSystem.levelTxt.text = $"{level}";
		tooltipSystem.healthTxt.text = $"{Mathf.Floor(Health)}";
		tooltipSystem.dmgTxt.text = $"{Mathf.Floor(stats.damage)}";

		if ((stats.attackSpeed * 10) > 40) tooltipSystem.atkSpdTxt.text = "Ass";
		else if ((stats.attackSpeed * 10) == 40) tooltipSystem.atkSpdTxt.text = "Slow";
		else if ((stats.attackSpeed * 10) == 20) tooltipSystem.atkSpdTxt.text = "Fast";
		else if ((stats.attackSpeed * 10) < 20) tooltipSystem.atkSpdTxt.text = "Nuts";

		tooltipSystem.digCountTxt.text = $"{Mathf.Floor(stats.digCount)}";
		
		tooltipSystem.unitDesc.text = stats.description;
		tooltipSystem.rarityTxt.text = rarity.ToString();
		SetLabelRarity(tooltipSystem.rarityLabel);

		tooltipSystem.blockChanceTxt.text = $"{Mathf.Floor(stats.blockChance * 100f)}";
		tooltipSystem.critChanceTxt.text = $"{Mathf.Floor(stats.critChance * 100f)}";

		TooltipSystem.Show(header);
	}

	public void OnPointerExit(PointerEventData eventData)
	{ 
		TooltipSystem.Hide();
	}

	private void SetLabelRarity(Image label)
	{
		label.color = rarity switch
		{
			UnitRarity.Common => (Color)new Color32(217, 217, 217, 255),
			UnitRarity.Rare => (Color)new Color32(128, 187, 245, 255),
			UnitRarity.Epic => (Color)new Color32(218, 128, 245, 255),
			UnitRarity.Legendary => (Color)new Color32(255, 175, 85, 255),
			_ => (Color)new Color32(217, 217, 217, 255),
		};
	}
}
