using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SlotTooltipTrigger : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
	private Slot unitSlot;

	private UnitStats stats;
	private float Health;
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
		Health = unitSlot.payload.GetComponent<Health>().maxHealth;
		rarity = stats.Rarity;

		// raw stats to show
		tooltipSystem.healthTxt.text = $"{Mathf.Ceil(Health)}";
		tooltipSystem.dmgTxt.text = $"{Mathf.Ceil(stats.attackPower)}";
		tooltipSystem.atkSpdTxt.text = $"{Mathf.Ceil(stats.attackInterval)}";
		tooltipSystem.digCountTxt.text = $"{Mathf.Ceil(stats.digCount)}";
		tooltipSystem.critDmgTxt.text = $"{Mathf.Ceil(stats.critDamage)}";
		
		tooltipSystem.unitDesc.text = stats.description;
		tooltipSystem.rarityTxt.text = rarity.ToString();
		SetLabelRarity(tooltipSystem.rarityLabel);

		// if the unit has no desc, hide the object.
		if(stats.description == null)
		{
			Debug.Log(tooltipSystem.unitDesc.gameObject.transform.parent.name);
		}

		// stats to show as percentage
		tooltipSystem.blockChanceTxt.text = $"{Mathf.Ceil(stats.blockChance * 100)}";
		
		if(stats.critChance >= 1)
		{
			stats.critChance = 1;
		}

		tooltipSystem.critChanceTxt.text = $"{Mathf.Ceil(stats.critChance * 100)}";

		// if the unit is in the battle slot, switch sides
		if (unitSlot.CompareTag("BattleSlot"))
		{
			TooltipSystem.Show(header, true);
		} else
		{
			TooltipSystem.Show(header, false);
		}
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
