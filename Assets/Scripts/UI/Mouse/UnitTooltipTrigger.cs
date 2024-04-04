using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UnitTooltipTrigger : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
	private MouseUtils cursor;

	private UnitStats stats;
	private float Health;
	private int level;
	private TooltipSystem tooltipSystem;

	private string header;

	private UnitRarity rarity;

	private void Start()
	{
		cursor = MouseUtils.singleton;
		tooltipSystem = TooltipSystem.instance;
		stats = transform.parent.GetComponent<UnitStats>();
		Health = transform.parent.GetComponent<Health>().maxHealth;
		level = transform.parent.GetComponent<Experience>().curLevel;
		header = transform.parent.name.Replace("(Clone)", "").Trim();
		rarity = stats.Rarity;
	}

	public void OnPointerEnter(PointerEventData eventData)
	{
		// raw stats to show
		tooltipSystem.levelTxt.text = $"{level}";
		tooltipSystem.healthTxt.text = $"{Mathf.Floor(Health)}";
		tooltipSystem.dmgTxt.text = $"{Mathf.Floor(stats.attackPower)}";
		tooltipSystem.atkSpdTxt.text = $"{Mathf.Floor(stats.attackInterval)}";
		tooltipSystem.digCountTxt.text = $"{Mathf.Floor(stats.digCount)}";
		tooltipSystem.critDmgTxt.text = $"{Mathf.Floor(stats.critDamage)}";
		
		tooltipSystem.unitDesc.text = stats.description;
		tooltipSystem.rarityTxt.text = rarity.ToString();
		SetLabelRarity(tooltipSystem.rarityLabel);
		
		//if the percentage is over 1, set it to 1
		if(stats.critChance >= 1)
		{
			stats.critChance = 1;
		}

		if (stats.blockChance >= 1)
		{
			stats.blockChance = 1;
		}

		tooltipSystem.blockChanceTxt.text = $"{Mathf.Floor(stats.blockChance * 100)}";
		tooltipSystem.critChanceTxt.text = $"{Mathf.Floor(stats.critChance * 100)}";

		TooltipSystem.Show(header);

		cursor.SetHoverCursor();
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		cursor.SetToDefaultCursor();
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
