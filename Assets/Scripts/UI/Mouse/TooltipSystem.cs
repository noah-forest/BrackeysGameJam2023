using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TooltipSystem : MonoBehaviour
{
	public static TooltipSystem instance;

	public Tooltip tooltip;

	public TextMeshProUGUI healthTxt;
	public TextMeshProUGUI dmgTxt;
	public TextMeshProUGUI blockChanceTxt;
	public TextMeshProUGUI critChanceTxt;
	public TextMeshProUGUI atkSpdTxt;
	public TextMeshProUGUI digCountTxt;
	public TextMeshProUGUI critDmgTxt;
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

	public static void Show(string header, bool side)
	{
		instance.tooltip.switchSide = side;

		instance.tooltip.SetText(header);
		instance.tooltip.gameObject.SetActive(true);
	}

	public static void Hide()
	{
		instance.tooltip.gameObject.SetActive(false);
	}
}
