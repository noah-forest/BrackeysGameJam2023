using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class SlotTooltipTrigger : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
	private Slot unitSlot;

	private UnitStats stats;
	private int Health;
	private TooltipSystem tooltipSystem;

	private string header;
	private string rarity;
	private Color rarityBackgroundColor;


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

		// raw stats to show
		tooltipSystem.healthTxt.text = Health.ToString();
		tooltipSystem.dmgTxt.text = $"{stats.attackPower}";
		tooltipSystem.atkSpdTxt.text = $"{stats.attackInterval}";
		tooltipSystem.digCountTxt.text = $"{stats.digCount}";
		tooltipSystem.critDmgTxt.text = $"{stats.critDamage}";
		tooltipSystem.unitDesc.text = stats.description;

		// if the unit has no desc, hide the object.
		if(stats.description == null)
		{
			Debug.Log(tooltipSystem.unitDesc.gameObject.transform.parent.name);
		}

		// stats to show as percentage
		tooltipSystem.blockChanceTxt.text = $"{stats.blockChance * 100}%";
		tooltipSystem.critChanceTxt.text = $"{stats.critChance * 100}%";

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
}
