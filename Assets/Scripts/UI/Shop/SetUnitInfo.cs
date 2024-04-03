using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SetUnitInfo : MonoBehaviour
{
	[HideInInspector] //this is set at runtime, do not manually set
	public Unit curUnit;

	public GameObject unitPreview;
	public Image shopPreviewImage;
	public Image shopPreviewShadow;
	public Image shopPreviewBackground;
	public Image shopLabel;
	public Image shopCostLabel;
	public Image shopLabelBorder;
	public Image shopFrame;

	public TextMeshProUGUI unitName;
	public TextMeshProUGUI unitCostText;

	[HideInInspector]
	public Button button; //this is set at runtime, do not manually set

	public GameObject purchased;
	public GameObject prefab;

	[HideInInspector]
	public int unitCost;

	//set this items information = to the info on the SO
	private void OnEnable()
	{
		if (curUnit == null) return;

		SetLabelRarity(shopLabel);

		shopLabelBorder.color = shopLabel.color;
		shopCostLabel.color = shopLabel.color;

		unitCost = (int)curUnit.unitRarity;

		shopPreviewImage.sprite = curUnit.itemPreview;
		shopPreviewShadow.sprite = shopPreviewImage.sprite;
		unitPreview.GetComponent<RectTransform>().anchoredPosition += curUnit.spriteOffset;

		shopPreviewBackground.color = curUnit.previewColor;

		unitName.SetText(curUnit.name);
		unitCostText.SetText(unitCost.ToString());
	}

	private void SetLabelRarity(Image label)
	{
		label.color = curUnit.unitRarity switch
		{
			UnitRarity.Common => (Color)new Color32(217, 217, 217, 255),
			UnitRarity.Rare => (Color)new Color32(128, 187, 245, 255),
			UnitRarity.Epic => (Color)new Color32(218, 128, 245, 255),
			UnitRarity.Legendary => (Color)new Color32(255, 175, 85, 255),
			_ => (Color)new Color32(217, 217, 217, 255),
		};
	}
} 
