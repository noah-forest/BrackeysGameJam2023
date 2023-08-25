using System.Collections.Generic;
using System.Linq;
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

	public GameManager gameManager;
	
	[HideInInspector]
	public MouseUtils mouseUtils;
	
	private List<GameObject> shopWindows = new List<GameObject>();
	private GameObject prefab;
	private GameObject shopWindow;
	private int unitIndex;
	private int posIndex;
	
	private void Start()
	{
		gameManager = GameManager.singleton;
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
			GameObject window = SetShopItems(unitPos[i].transform);
			shopWindows.Add(shopWindow);	// add it to a list of instantiated objects
		}
		
		mouseUtils.FindButtonsInScene();
	}
	
	// loop through the SOs and their prefabs to set certain information
	private GameObject SetShopItems(Transform parent)
	{
		for (int i = 0; i < shopItems.Count; i++)
		{
			unitIndex = Random.Range(0, shopItems.Count);
		}
		
		prefab = shopItems[unitIndex].prefab;
		shopWindow = Instantiate(prefab, parent);
		
		SetUnitInfo setUnitInfo = shopWindow.GetComponent<SetUnitInfo>();
		Slot unitSlot = shopWindow.GetComponent<Slot>();
		
		unitSlot.AddDropPrecheck((slot, shopSlot) => {
			if (slot.payload != null)
			{
				// sold the unit
				slot.payload = null;
				gameManager.Gold += 2;
			}
			return false;
		});
		
		unitSlot.AddRetrievePrecheck((shopSlot, newSlot) =>
		{
			if (newSlot.payload != null)
			{
				return false;
			};
			
			if (gameManager.Gold >= 3)
			{
				// bought the unit
				setUnitInfo.purchased.gameObject.SetActive(true);
				gameManager.Gold -= 3;
				return true;
			}
			
			return false;
		});
		
		setUnitInfo.unitName.SetText(shopItems[unitIndex].name);
		setUnitInfo.unitCost.SetText("3");

		return shopWindow;
	}
}
