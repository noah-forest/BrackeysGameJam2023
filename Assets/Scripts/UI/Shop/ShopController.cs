using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class ShopController : MonoBehaviour
{
	public List<ShopItem> shopItems = new List<ShopItem>();
	public List<GameObject> unitPos = new List<GameObject>();
	
	public GameObject sellWindow;

	[HideInInspector]
	public GameManager gameManager;
	
	[HideInInspector]
	public MouseUtils mouseUtils;

	private ShopAudio shopAudioPlayer;
	
	private List<GameObject> shopWindows = new List<GameObject>();
	private GameObject prefab;
	private GameObject shopWindow;
	private int unitIndex;
	private int posIndex;
	
	private void Start()
	{
		gameManager = GameManager.singleton;
		mouseUtils = MouseUtils.singleton;

		shopAudioPlayer = GetComponent<ShopAudio>();
		
		Slot.anyDragStarted.AddListener(arg0 =>
		{
			shopAudioPlayer.PlayAudioClipOnce(shopAudioPlayer.audioClips[1]);
			if (arg0.CompareTag("BattleSlot") || arg0.CompareTag("ReserveSlot"))
			{
				sellWindow.SetActive(true);
			}
		});
		
		Slot.anyDragStopped.AddListener(arg0 =>
		{
			if (arg0.CompareTag("BattleSlot") || arg0.CompareTag("ReserveSlot"))
			{
				shopAudioPlayer.PlayAudioClipOnce(shopAudioPlayer.audioClips[2]);
				sellWindow.SetActive(false);
			}
			else
			{
				shopAudioPlayer.PlayAudioClipOnce(shopAudioPlayer.audioClips[0]);
			}
		});
		
		Slot unitSlot = sellWindow.GetComponent<Slot>();
		
		unitSlot.AddDropPrecheck((slot, shopSlot) => {
			if (slot.payload != null)
			{
				// sold the unit
				slot.payload = null;
				shopAudioPlayer.PlayAudioClipOnce(shopAudioPlayer.audioClips[3]);
				gameManager.Gold += 2;
			}
			return false;
		});
		
		PopulateShopUnits();
	}

	private void OnEnable()
	{
		
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
		
		unitSlot.AddRetrievePrecheck((shopSlot, newSlot) =>
		{
			if (newSlot.payload != null)
			{
				shopAudioPlayer.PlayAudioClipOnce(shopAudioPlayer.audioClips[0]);
				return false;
			};
			
			if (gameManager.Gold >= 3)
			{
				// bought the unit
				setUnitInfo.purchased.gameObject.SetActive(true);
				gameManager.Gold -= 3;
				shopAudioPlayer.PlayAudioClipOnce(shopAudioPlayer.audioClips[2]);
				return true;
			}
			
			return false;
		});
		
		setUnitInfo.unitName.SetText(shopItems[unitIndex].name);
		setUnitInfo.unitCost.SetText("3");

		return shopWindow;
	}
}
