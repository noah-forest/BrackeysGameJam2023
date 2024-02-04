using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TooltipSystem : MonoBehaviour
{
	private static TooltipSystem instance;

	public Tooltip tooltip;

	public void Awake()
	{
		instance = this;
		Hide();
	}

	public static void Show(string content, string header = "")
	{
		instance.tooltip.SetText(content, header);
		instance.tooltip.gameObject.SetActive(true);
	}

	public static void Hide()
	{
		instance.tooltip.gameObject.SetActive(false);
	}
}
