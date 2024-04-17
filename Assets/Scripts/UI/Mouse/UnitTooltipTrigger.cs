using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UnitTooltipTrigger : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
	private MouseUtils cursor;

	private UnitStats stats;
	private Health Health;
	private int level;
	private TooltipSystem tooltipSystem;

	private string header;

	private UnitRarity rarity;

	private void Start()
	{
		cursor = MouseUtils.singleton;
		tooltipSystem = TooltipSystem.instance;
		stats = transform.parent.GetComponent<UnitStats>();
		Health = transform.parent.GetComponent<Health>();
		Health.died.AddListener(HideTooltip);
		header = transform.parent.name.Replace("(Clone)", "").Trim();
		rarity = stats.Rarity;
	}

	public void OnPointerEnter(PointerEventData eventData)
	{
		level = transform.parent.GetComponent<Experience>().curLevel;

		// raw stats to show
		tooltipSystem.levelTxt.text = $"{level}";
		tooltipSystem.healthTxt.text = $"{Mathf.Floor(Health.maxHealth)}";
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

		cursor.SetHoverCursor();
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		HideTooltip();
	}

	public void HideTooltip()
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
