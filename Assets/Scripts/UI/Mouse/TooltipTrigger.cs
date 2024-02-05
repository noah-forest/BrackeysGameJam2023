using UnityEngine;
using UnityEngine.EventSystems;

public class TooltipTrigger : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
	public SetUnitInfo unitInfo;

	private UnitStats stats;
	private Slot unitSlot;

	private GameObject prefab;

	private string header;
	public string content;

	private void Start()
	{
		header = unitInfo.curUnit.unitName;
	}

	public void OnPointerEnter(PointerEventData eventData)
	{
		prefab = unitInfo.prefab;
		stats = prefab.GetComponent<UnitStats>();
		content = $"Damage: {stats.attackPower}\n"
			+ $"Attack Speed: {stats.attackInterval}\n"
			+ $"Block Chance: {stats.blockChance}\n"
			+ $"Crit Chance: {stats.critChance}\n"
			+ $"Crit Damage: {stats.critDamage}\n"
			+ $"Digs to Revive: {stats.digCount}\n";
		TooltipSystem.Show(content, header);
	}

	public void OnPointerExit(PointerEventData eventData)
	{ 
		TooltipSystem.Hide();
	}
}
