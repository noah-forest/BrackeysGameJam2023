using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopManager : MonoBehaviour
{
	public Canvas canvas;

	public List<ShopItem> shopItems = new List<ShopItem>();
	public List<GameObject> unitPos = new List<GameObject>();
	public List<GameObject> itemPos = new List<GameObject>();
    
	private void Start()
	{
		SetShopItems();
	}

	private void SetShopItems()
	{
		for (int i = 0; i < shopItems.Count; i++)
		{
			
		}
	}
}
