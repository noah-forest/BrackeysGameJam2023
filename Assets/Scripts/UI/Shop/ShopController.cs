using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class ShopController : MonoBehaviour
{
	public List<ShopItem> shopItems = new List<ShopItem>();
	public List<GameObject> unitPos = new List<GameObject>();

	public LootTable lootTable;

	public MouseUtils mouseUtils;
	
	private List<GameObject> shopWindows = new List<GameObject>();
	private GameObject prefab;
	private GameObject shopWindow;
	private int unitIndex;
	private int posIndex;
	
	private void Start()
	{
		mouseUtils = MouseUtils.singleton;
		PopulateShopUnits();
	}
	
	/// <summary>
	/// Populates the shop with units
	/// </summary>
	public void PopulateShopUnits()
	{
		// if the shop items are already being displayed
		if (shopWindows.Count != 0)
		{
			for (int i = 0; i < shopWindows.Count; i++)
			{
				Destroy(shopWindows[i].gameObject);		// delete them
			}
			shopWindows.Clear();		// clear the list
		}
		
		// loop through the unit positions and place a new shop item there
		for (int i = 0; i < unitPos.Count; i++)
		{
			SetShopItems();
			shopWindow = Instantiate(prefab, unitPos[i].transform);
			shopWindows.Add(shopWindow);	// add it to a list of instantiated objects
		}
		
		mouseUtils.FindButtonsInScene();
	}
	
	// loop through the SOs and their prefabs to set certain information
	private void SetShopItems()
	{
		for (int i = 0; i < shopItems.Count; i++)
		{
			unitIndex = Random.Range(0, shopItems.Count);
		}
        
		/* rarity is not working rn 
		switch (lootTable.GetRarity().rarityName)
		{
			case "Common":
				if (shopItems[unitIndex].rarity != "Common") return;
				break;
			case "Rare":
				if (shopItems[unitIndex].rarity != "Rare") return;
				break;
			case "Epic":
				if (shopItems[unitIndex].rarity != "Epic") return;
				break;
			case "Legendary":
				if (shopItems[unitIndex].rarity != "Legendary") return;
				break;
		} */
		
		prefab = shopItems[unitIndex].prefab;
		SetUnitInfo setUnitInfo = prefab.GetComponent<SetUnitInfo>();
        
		setUnitInfo.unitName.SetText(shopItems[unitIndex].name);
		setUnitInfo.unitCost.SetText("3");
	}
}
