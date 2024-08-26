using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TooltipSystem : MonoBehaviour
{
    private static TooltipSystem instance;
    
	public Tooltip tooltip;

	private void Awake()
	{
		instance = this;
	}

	public static void Show(string content)
	{
		instance.tooltip.gameObject.SetActive(true);
		instance.tooltip.contentText.SetText(content);
	}

	public static void Hide()
	{
		instance.tooltip.gameObject.SetActive(false);
	}
}
