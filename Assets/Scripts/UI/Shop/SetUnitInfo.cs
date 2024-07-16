using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SetUnitInfo : MonoBehaviour
{
	private GameManager gameManager;

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
	public UnitStats unitStats;
	
	private UnitRarity rarity;
	public List<GameObject> rarityGlowList = new();

	public GameObject unitFound;
	public GameObject unitShine;

	[HideInInspector]
	public int unitCost;

	private void Awake()
	{
		gameManager = GameManager.singleton;
	}

	//set this items information = to the info on the SO
	private void OnEnable()
	{
		unitStats = curUnit.unitStats;
		rarity = unitStats.Rarity;

		if (curUnit == null) return;

		SetLabelRarity(shopLabel);

		shopLabelBorder.color = shopLabel.color;
		shopCostLabel.color = shopLabel.color;

		if(rarity == UnitRarity.Legendary)
		{
			unitCost = 7;
		} else
		{
			unitCost = (int)rarity;
		}

		if (Settings.RarityBorders)
		{
			foreach (GameObject item in rarityGlowList)
			{
				if (item.name == rarity.ToString())
				{
					item.SetActive(true);
				}
			}
		}

		shopPreviewImage.sprite = curUnit.itemPreview;
		shopPreviewShadow.sprite = shopPreviewImage.sprite;
		unitPreview.GetComponent<RectTransform>().anchoredPosition += curUnit.spriteOffset;

		shopPreviewBackground.color = curUnit.previewColor;

		unitName.SetText(curUnit.name);
		unitCostText.SetText(unitCost.ToString());
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
