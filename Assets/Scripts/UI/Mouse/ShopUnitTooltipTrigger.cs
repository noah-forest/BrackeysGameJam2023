using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class ShopUnitTooltipTrigger : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
	public SetUnitInfo unitInfo;

	private UnitStats stats;
	private int Health;
	private TooltipSystem tooltipSystem;

	private GameObject prefab;

	private string header;
	private string rarity;
	private Color rarityBackgroundColor;


	private void Start()
	{
		tooltipSystem = TooltipSystem.instance;
		header = unitInfo.curUnit.unitName;
	}

	public void OnPointerEnter(PointerEventData eventData)
	{
		prefab = unitInfo.prefab;
		stats = prefab.GetComponent<UnitStats>();
		Health = prefab.GetComponent<Health>().maxHealth;
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

		tooltipSystem.SwitchTooltipSide();

		TooltipSystem.Show(header);
	}

	public void OnPointerExit(PointerEventData eventData)
	{ 
		TooltipSystem.Hide();
	}
}
