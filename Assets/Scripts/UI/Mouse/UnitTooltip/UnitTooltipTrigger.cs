using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UnitTooltipTrigger : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
	private MouseUtils cursor;

	private UnitStats stats;
	private Health health;
	private int level;
	private UnitTooltipSystem tooltipSystem;

	private string header;

	private UnitRarity rarity;

	private bool hovering;

	private void Start()
	{
		cursor = MouseUtils.singleton;
		tooltipSystem = UnitTooltipSystem.instance;
		stats = transform.parent.GetComponent<UnitStats>();
		health = transform.parent.GetComponent<Health>();
		health.unitDied.AddListener(UnitDied);
		header = transform.parent.name.Replace("(Clone)", "").Trim();
		rarity = stats.Rarity;
	}

	private void UnitDied(GameObject go)
	{
		if(go == transform.parent.gameObject && hovering)
		{
			HideTooltip();
		}
	}

	public void OnPointerEnter(PointerEventData eventData)
	{
		hovering = true;

		level = transform.parent.GetComponent<Experience>().curLevel;

		// raw stats to show
		tooltipSystem.levelTxt.text = $"{level}";
		tooltipSystem.healthTxt.text = $"{(float)health.maxHealth}";
		tooltipSystem.dmgTxt.text = $"{(float)stats.damage}";

		if ((stats.attackSpeed * 10) > 40) tooltipSystem.atkSpdTxt.text = "Booty";
		else if ((stats.attackSpeed * 10) == 40) tooltipSystem.atkSpdTxt.text = "Slow";
		else if ((stats.attackSpeed * 10) == 20) tooltipSystem.atkSpdTxt.text = "Average";
		else if ((stats.attackSpeed * 10) < 10) tooltipSystem.atkSpdTxt.text = "Nuts";
		else if ((stats.attackSpeed * 10) < 20) tooltipSystem.atkSpdTxt.text = "Fast";
		

		tooltipSystem.digCountTxt.text = $"{(int)stats.digCount}";
		
		tooltipSystem.unitDesc.text = stats.description;
		tooltipSystem.rarityTxt.text = rarity.ToString();
		SetLabelRarity(tooltipSystem.rarityLabel);

		tooltipSystem.blockChanceTxt.text = $"{stats.blockChance * 100f}";
		tooltipSystem.critChanceTxt.text = $"{stats.critChance * 100f}";

		UnitTooltipSystem.Show(header);

		cursor.SetTooltipCursor();
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		hovering = false;
		HideTooltip();
	}

	public void HideTooltip()
	{
		cursor.SetToDefaultCursor();
		UnitTooltipSystem.Hide();
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
