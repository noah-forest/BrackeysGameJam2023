using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UnitTooltipSystem : MonoBehaviour
{
	public static UnitTooltipSystem instance;

	public UnitTooltip tooltip;

	public TextMeshProUGUI levelTxt;
	public TextMeshProUGUI healthTxt;
	public TextMeshProUGUI dmgTxt;
	public TextMeshProUGUI blockChanceTxt;
	public TextMeshProUGUI critChanceTxt;
	public TextMeshProUGUI atkSpdTxt;
	public TextMeshProUGUI digCountTxt;
	public TextMeshProUGUI rarityTxt;
	public Image rarityLabel;
	public TextMeshProUGUI unitDesc;

	private void Awake()
	{
		if (instance)
		{
			Destroy(this.gameObject);
			return;
		}

		instance = this;
		Hide();
	}

	public static void Show(string header)
	{
		instance.tooltip.SetText(header);
		instance.tooltip.gameObject.SetActive(true);
	}

	public static void Hide()
	{
		instance.tooltip.gameObject.SetActive(false);
	}
}
