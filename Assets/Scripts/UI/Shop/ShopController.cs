using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopController : MonoBehaviour
{
	public List<ShopItem> shopItems = new List<ShopItem>();
	public List<GameObject> unitPos = new List<GameObject>();
	public List<GameObject> itemPos = new List<GameObject>();

	private GameObject prefab;
	private GameObject shopWindow;
	private int unitIndex;
	private int posIndex;
	
	private void Start()
	{
		PopulateShop();
	}
	
	public void PopulateShop()
	{
		if(shopWindow != null) Destroy(shopWindow.gameObject);
		for (int i = 0; i < unitPos.Count; i++)
		{
			SetShopItems();
			shopWindow = Instantiate(prefab, unitPos[i].transform) as GameObject;
		}
	}
	
	private void SetShopItems()
	{
		for (int i = 0; i < shopItems.Count; i++)
		{
			unitIndex = Random.Range(0, shopItems.Count);
		}
		
		prefab = shopItems[unitIndex].prefab;
		SetUnitInfo setUnitInfo = prefab.GetComponent<SetUnitInfo>();
            
		setUnitInfo.unitName.SetText(shopItems[unitIndex].name);
		setUnitInfo.unitCost.SetText("5");
	}
}
