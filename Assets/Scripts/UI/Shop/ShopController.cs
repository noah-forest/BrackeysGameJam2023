using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public struct RarityTable
{
	public Dictionary<UnitRarity, int> rarityWeights;
	public int weightedTotal;

	public RarityTable(int commonWeight, int rareWeight, int epicWeight, int legendaryWeight)
	{
		weightedTotal = commonWeight + rareWeight + epicWeight + legendaryWeight;
		rarityWeights = new Dictionary<UnitRarity, int>
		{
			{ UnitRarity.Common, commonWeight },
			{ UnitRarity.Rare, rareWeight },
			{ UnitRarity.Epic, epicWeight },
			{ UnitRarity.Legendary, legendaryWeight }
		};
	}
}

public class ShopController : MonoBehaviour
{
	// this is for the UI
	public List<GameObject> shopItemPos = new();
	private List<GameObject> shopWindows = new();

	// lists to populate from resources
	private List<Unit> shopUnits = new();

	public Button refreshButton;
	public GameObject sellWindow;
	public TextMeshProUGUI sellWindowPrice;

	public StatLegend legend;
	private GameManager gameManager;
	private SaveData saveData;
	private BattleManager battleManager;
	private ShopAudio shopAudioPlayer;

	private GameObject shopItem;
	private GameObject shopWindow;

	private int unitIndex;
	public int refreshCost;

	private SetUnitInfo curUnitInfo;

	private bool draggedIntoShop;

	public bool firstRoll;          // this is to check if the shop has been initially rolled
	public bool unitInInventory;

	private bool unitShine;
	private bool legendWasOpen;
	private bool canAfford;

	private RarityTable defaultRarities;

	[Space(15)]
	[Header("Rarity Weights")]
	public int commonWeight;
	public int rareWeight;
	public int epicWeight;
	public int legendaryWeight;

	private void Start()
	{
		battleManager = BattleManager.singleton;
		gameManager = GameManager.singleton;
		saveData = SaveData.singleton;
		gameManager.unitAddedToSlot.AddListener(SearchAfterPurchase);
		gameManager.unitSold.AddListener(SearchAfterPurchase);
		shopAudioPlayer = GetComponent<ShopAudio>();

		defaultRarities = new RarityTable(commonWeight, rareWeight, epicWeight, legendaryWeight);

		LoadResources();        // load the resources from the game files

		firstRoll = false;

		refreshCost = 1;

		#region delegates
		Slot.anyDragStarted.AddListener(arg0 =>
		{
			if (arg0.CompareTag("BattleSlot") || arg0.CompareTag("ReserveSlot"))
			{
				sellWindow.SetActive(true);
			}

			if (arg0.CompareTag("ShopSlot"))
			{
				SetUnitInfo unitInfo = arg0.GetComponent<SetUnitInfo>();
				if (unitInfo != null)
				{
					unitInfo.shopPreviewImage.gameObject.SetActive(false);
				}
			}

			if (arg0.payload != null)
			{
				shopAudioPlayer.PlayAudioClipOnce(shopAudioPlayer.audioClips[1]);
				UnitStats sellInfo = arg0.payload.GetComponent<UnitStats>();
				sellWindowPrice.text = $"{sellInfo.sellValue.Value}";
				//Debug.Log("playing pick up sound");
			}

			switch (legend.open)
			{
				case true:
					legendWasOpen = true;
					legend.CloseStatLegend();
					break;
				case false:
					legendWasOpen = false;
					break;
			}
		});

		Slot.anyDragStopped.AddListener(arg0 =>
		{
			if (legendWasOpen)
			{
				legend.OpenStatLegend();
			}

			if (arg0.payload != null && !draggedIntoShop)
			{
				shopAudioPlayer.PlayAudioClipOnce(shopAudioPlayer.audioClips[2]);
				//Debug.Log("playing drop sound");
			}

			if (arg0.CompareTag("BattleSlot") || arg0.CompareTag("ReserveSlot"))
			{
				sellWindow.SetActive(false);
			}

			if (!arg0.CompareTag("ShopSlot")) return;
			SetUnitInfo unitInfo = arg0.GetComponent<SetUnitInfo>();
			if (unitInfo != null)
			{
				unitInfo.shopPreviewImage.gameObject.SetActive(true);
			}

		});
		
		Slot.controlClicked.AddListener(arg0 =>
		{
			// if you control + click a shop slot it will auto purchase the unit
			if (arg0.CompareTag("ShopSlot"))
			{
				SetUnitInfo unitInfo = arg0.GetComponent<SetUnitInfo>();
				canAfford = gameManager.Cash >= unitInfo.unitCost;
				if (unitInfo != null && canAfford && arg0.payload)
				{
					//find an available slot
					Slot slot = FindNearestEmptySlot(battleManager.playerBattleSlots);
					
					if (!slot && FindNearestEmptySlot(battleManager.playerReserveSlots))
					{
						slot = FindNearestEmptySlot(battleManager.playerReserveSlots);
					}
					else if(!slot)
					{
						return;
					}
					
					// purchase the unit
					PurchaseUnit(slot, unitInfo);
					slot.quickAction.Invoke();
					slot.payload = arg0.payload;
					arg0.payload = null;
				}
			}
			
			// if you control + click a inv slot it will auto sell the unit
			if (arg0.CompareTag("BattleSlot") || arg0.CompareTag("ReserveSlot"))
			{
				if (arg0.payload)
				{
					SellUnit(arg0);
					arg0.quickAction.Invoke();
				}
			}
		});
		
		Slot unitSlot = sellWindow.GetComponent<Slot>();

		unitSlot.AddDropPrecheck((slot, shopSlot) =>
		{
			if (slot.payload != null)
			{
				// sold the unit
				SellUnit(slot);
			}
			return false;
		});
		#endregion

		PopulateShopUnits();
	}
	
	/// <summary>
	/// Populates the shop with units
	/// </summary>
	public void PopulateShopUnits()
	{
		// loop through the unit positions and place a new shop item there
		foreach (GameObject unitPos in shopItemPos)
		{
			SetShopItem(unitPos.transform);

			SetUnitInfo curShopItem = shopWindow.GetComponent<SetUnitInfo>();
			Slot unitSlot = shopWindow.GetComponent<Slot>();
			curShopItem.unitFound.SetActive(false);
			curShopItem.unitShine.SetActive(false);

			SetUnitPayload(unitSlot, curShopItem);
			UnitFoundInInventory(unitSlot, curShopItem);
			unitShine = true;

			UnitStats sellInfo = unitSlot.payload.GetComponent<UnitStats>();
			sellInfo.sellValue.BaseValue = (int)(curShopItem.unitCost * 0.75);

			unitSlot.AddRetrievePrecheck((shopSlot, newSlot) =>
			{
				canAfford = gameManager.Cash >= curShopItem.unitCost;
				
				if (newSlot.gameObject.name.Contains("Shop"))
				{
					return false;
				}

				//moves unit in battleSlot to nearest empty reserve slot
				if (newSlot.payload != null && newSlot.payload.name != unitSlot.payload.name && canAfford)
				{
					Slot slot = FindNearestEmptySlot(battleManager.playerReserveSlots);

					if (slot != null)
					{
						slot.payload = newSlot.payload;
						newSlot.payload = null;
					}
					else
					{
						return false;
					}
				}

				if (canAfford)
				{
					PurchaseUnit(newSlot, curShopItem);
					return true;
				}
				
				return false;
			});

			shopWindows.Add(shopWindow);
		}
		firstRoll = true;
	}

	private void SellUnit(Slot slot)
	{
		UnitStats sellInfo = slot.payload.GetComponent<UnitStats>();
		draggedIntoShop = true;
		slot.payload = null;
		shopAudioPlayer.PlayAudioClipOnce(shopAudioPlayer.audioClips[3]);
		gameManager.Cash += (int)sellInfo.sellValue.Value;
		Destroy(slot.payload);
		gameManager.unitSold.Invoke(slot);
	}
	
	private void PurchaseUnit(Slot newSlot, SetUnitInfo curShopItem)
	{
		if (newSlot.payload != null)
		{
			Experience unitExp = newSlot.payload.GetComponent<Experience>();
			if (unitExp && unitExp.curLevel == Experience.MaxLevel) return;
		}

		curShopItem.unitPreview.SetActive(false);
		curShopItem.purchased.SetActive(true);
					
		//turn off the rarity glow on units on purchase
		foreach (var glow in curShopItem.rarityGlowList.Where(glow => glow.activeInHierarchy))
		{
			glow.SetActive(false);
		}
		
		curShopItem.unitFound.SetActive(false);
		curShopItem.unitShine.SetActive(false);
		gameManager.Cash -= curShopItem.unitCost;
		gameManager.unitPurchased?.Invoke();

		//unlock the unit in the logbook
		UnlockUnit(curShopItem);

		shopAudioPlayer.PlayAudioClipOnce(shopAudioPlayer.audioClips[3]);
	}

	private void UnlockUnit(SetUnitInfo curShopItem)
	{
		var unitToUnlock = curShopItem.curUnit.name;
		
		// check to make sure the unit hasn't already been added, then add it
		if (saveData.unlockMatrix.unitsUnlocked.Contains(unitToUnlock)) return;
		saveData.unlockMatrix.unitsUnlocked.Add(unitToUnlock);
		saveData.SaveIntoJson();

		PopulateEntries.unlockedEntriesChanged.Invoke();

		// sends event to unlock the unit in the logbook
		EntryInfo.onEntryUnlocked.Invoke(unitToUnlock);
	}
	
	private void SearchAfterPurchase(Slot slot)
	{
		foreach (var window in shopWindows)
		{
			SetUnitInfo curShopItem = window.GetComponent<SetUnitInfo>();
			Slot unitSlot = window.GetComponent<Slot>();
			curShopItem.unitFound.SetActive(false); //set this to false before searching, effectively clearing it
			curShopItem.unitShine.SetActive(false);

			if (unitSlot.payload != null)
			{
				UnitFoundInInventory(unitSlot, curShopItem);
			}
		}
	}

	private void UnitFoundInInventory(Slot unitSlot, SetUnitInfo curShopItem)
	{
		SearchThroughSlots(unitSlot, curShopItem, battleManager.playerBattleSlots);
		SearchThroughSlots(unitSlot, curShopItem, battleManager.playerReserveSlots);
	}

	private void SearchThroughSlots(Slot unitSlot, SetUnitInfo curShopItem, List<Slot> slots)
	{
		foreach (Slot slot in slots)
		{
			if (slot.payload == null) continue;
			if (slot.payload.GetComponent<Experience>().curLevel == Experience.MaxLevel) continue;
			if (slot.payload.name != unitSlot.payload.name) continue;
			//if its appearing in the shop for the first time
			if (unitShine)
			{
				Color defaultColor = curShopItem.shine.color;
				curShopItem.shine.color = defaultColor;

				//change the shine color to orange if its a legendary in shop
				if (curShopItem.unitStats.Rarity == UnitRarity.Legendary)
				{
					curShopItem.shine.color = new Color32(255, 164, 0, 255);
				}

				StartCoroutine(PlayShineAnim(curShopItem));
			}

			//if you cant afford it, dont show the glow
			if (gameManager.Cash < curShopItem.unitCost) continue;

			//show the glow
			curShopItem.unitFound.SetActive(true);
		}
	}

	// plays the shine animation, and then sets unitShine to false
	private IEnumerator PlayShineAnim(SetUnitInfo curShopItem)
	{
		curShopItem.unitShine.SetActive(true);

		Animator anim = curShopItem.unitShine.GetComponent<Animator>();
		anim.SetTrigger("unitFound");

		yield return new WaitForSeconds(0.2f);

		unitShine = false;
	}

	private static Slot FindNearestEmptySlot(IEnumerable<Slot> slots)
	{
		return slots.FirstOrDefault(slot => slot.payload == null);
	}

	public void ClearShopWindows()
	{
		// if the shop items are already being displayed
		if (shopWindows.Count == 0) return;
		foreach (GameObject window in shopWindows)
		{
			if (window.GetComponent<Slot>().payload != null) Destroy(window.GetComponent<Slot>().payload);
			Destroy(window);
		} // delete them
		shopWindows.Clear(); // clear the list
		shopWindows.TrimExcess(); // wipe the memory
	}

	//loops through the folders containing the SOs
	//	grabs the corresponding prefab according to name
	//		and sets it to the payload
	private void SetUnitPayload(Slot curUnitSlot, SetUnitInfo curShopItem)
	{
		foreach (Unit unit in shopUnits)
		{
			for (int i = 0; i < battleManager.unitManager.unitStatsDatabase.Count; i++)
			{
				if (curShopItem.unitName.text != unit.name ||
				    curShopItem.unitName.text != battleManager.unitManager.unitStatsDatabase[i].name) continue;
				curUnitSlot.payload = battleManager.CreateUnitInstance(i, transform);
				curUnitSlot.payload.SetActive(false);
			}
		}
	}

	// sets a single shop item at a certain location
	private void SetShopItem(Transform parent)
	{
		var unitInfo = battleManager.unitManager.unitStatsDatabase[GetRandomUnitWeighted(defaultRarities)];

		foreach (var unit in shopUnits)
		{
			FindShopItem(); // find shopitem prefab and inits the curUnitInfo
			if (unit.name == unitInfo.name)
			{
				curUnitInfo.curUnit = unit;
			}
		}

		shopWindow = Instantiate(shopItem, parent); // creates the window object (1 of 4)
	}

	private UnitRarity RollRarity(RarityTable rarityTable)
	{
		int roll = Random.Range(0, rarityTable.weightedTotal);
		foreach (KeyValuePair<UnitRarity, int> kvp in rarityTable.rarityWeights)
		{
			if (roll <= kvp.Value)
			{
				return kvp.Key;
			}
			else
			{
				roll -= kvp.Value;
			}
		}

		return UnitRarity.Common;
	}

	private int GetRandomUnitWeighted(RarityTable rarityTable)
	{
		UnitRarity rarity = RollRarity(rarityTable);

		UnitManager unitManager = battleManager.unitManager;
		Dictionary<UnitRarity, int> rarityOffsets = unitManager.rarityOffsets;
		Dictionary<UnitRarity, int> unitRarityCount = unitManager.unitRarityCount;

		int roll = Random.Range(rarityOffsets[rarity], rarityOffsets[rarity] + unitRarityCount[rarity]);
		return roll;
	}

	private void FindShopItem()
	{
		shopItem = Resources.Load("Prefabs/unitShopItem") as GameObject;
		if (shopItem != null)
		{
			curUnitInfo = shopItem.GetComponent<SetUnitInfo>();
		}
	}

	// populates the lists used for units and unit prefabs
	private void LoadResources()
	{
		shopUnits = Resources.LoadAll<Unit>("SOs/Units").ToList();
	}
}