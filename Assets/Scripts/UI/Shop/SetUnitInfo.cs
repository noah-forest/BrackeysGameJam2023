using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SetUnitInfo : MonoBehaviour
{
	[HideInInspector] //this is set at runtime, do not manually set
	public Unit curUnit;

	public Image shopPreviewImage;
	public Image shopPreviewBackground;

	public TextMeshProUGUI unitName;
	public TextMeshProUGUI unitCostText;

	[HideInInspector]
	public Button button; //this is set at runtime, do not manually set

	public GameObject purchased;

	[HideInInspector]
	public int unitCost;

	//set this items information = to the info on the SO
	private void OnEnable()
	{
		if (curUnit == null) return;

		unitCost = (int)curUnit.unitRarity;

		shopPreviewImage.sprite = curUnit.itemPreview;

		shopPreviewImage.GetComponent<RectTransform>().anchoredPosition += curUnit.spriteOffset;

		shopPreviewBackground.color = curUnit.previewColor;
		unitName.SetText(curUnit.name);
		unitCostText.SetText(unitCost.ToString());
	}
}
