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
	private BattleManager battleManager;
	private ShopAudio shopAudioPlayer;

	private GameObject shopItem;
	private GameObject shopWindow;

	private int unitIndex;
	public int refreshCost;

	private SetUnitInfo curUnitInfo;

	private bool draggedIntoShop;

	public bool firstRoll;          // this is to check if the shop has been initially rolled
	public bool unitInInventory = false;

	private bool unitShine;
	private bool legendWasOpen;

	public RarityTable defaultRarities;

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

			if (legend.open)
			{
				legendWasOpen = true;
				legend.CloseStatLegend();
			}
			else if (!legend.open)
			{
				legendWasOpen = false;
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

			if (arg0.CompareTag("ShopSlot"))
			{
				SetUnitInfo unitInfo = arg0.GetComponent<SetUnitInfo>();
				if (unitInfo != null)
				{
					unitInfo.shopPreviewImage.gameObject.SetActive(true);
				}
			}

		});

		Slot unitSlot = sellWindow.GetComponent<Slot>();

		unitSlot.AddDropPrecheck((slot, shopSlot) =>
		{
			if (slot.payload != null)
			{
				// sold the unit
				UnitStats sellInfo = slot.payload.GetComponent<UnitStats>();
				draggedIntoShop = true;
				slot.payload = null;
				shopAudioPlayer.PlayAudioClipOnce(shopAudioPlayer.audioClips[3]);
				gameManager.Cash += (int)sellInfo.sellValue.Value;
				Destroy(slot.payload);
				gameManager.unitSold.Invoke();
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
				if (newSlot.gameObject.name.Contains("Shop"))
				{
					return false;
				}

				//moves unit in battleSlot to nearest empty reserve slot
				if (newSlot.payload != null && newSlot.payload.name != unitSlot.payload.name)
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
				};

				//unit purchase check
				if (gameManager.Cash >= curShopItem.unitCost)
				{
					if (newSlot.payload != null)
					{
						Experience unitExp = newSlot.payload.GetComponent<Experience>();
						if (unitExp && unitExp.curLevel == Experience.MaxLevel) return false;
					}

					curShopItem.purchased.SetActive(true);
					
					//turn off the rarity glow on units on purchase
					foreach(GameObject glow in curShopItem.rarityGlowList)
					{
						if (glow.activeInHierarchy)
						{
							glow.SetActive(false);
						}
					}

					curShopItem.unitFound.SetActive(false);
					curShopItem.unitShine.SetActive(false);
					gameManager.Cash -= curShopItem.unitCost;
					gameManager.unitPurchased?.Invoke();

					shopAudioPlayer.PlayAudioClipOnce(shopAudioPlayer.audioClips[3]);
					return true;
				}
				return false;
			});

			shopWindows.Add(shopWindow);
		}
		firstRoll = true;
	}

	public void SearchAfterPurchase()
	{
		for (int i = 0; i < shopWindows.Count;  i++)
		{
			SetUnitInfo curShopItem = shopWindows[i].GetComponent<SetUnitInfo>();
			Slot unitSlot = shopWindows[i].GetComponent<Slot>();
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
		foreach(Slot slot in battleManager.playerBattleSlots)
		{
			if (slot.payload == null) continue;

			if (slot.payload.GetComponent<Experience>().curLevel == Experience.MaxLevel) continue;
			if (slot.payload.name == unitSlot.payload.name)
			{
				curShopItem.unitFound.SetActive(true);
				if (unitShine)
				{
					StartCoroutine(playShineAnim(curShopItem));
				}
			}
		}

		foreach (Slot slot in battleManager.playerReserveSlots)
		{
			if (slot.payload == null) continue;
			if (slot.payload.GetComponent<Experience>().curLevel == Experience.MaxLevel) continue;
			if (slot.payload.name == unitSlot.payload.name)
			{
				curShopItem.unitFound.SetActive(true);
				if (unitShine)
				{
					StartCoroutine(playShineAnim(curShopItem));
				}
			}
		}
	}

	private IEnumerator playShineAnim(SetUnitInfo curShopItem)
	{
		curShopItem.unitShine.SetActive(true);

		Animator anim = curShopItem.unitShine.GetComponent<Animator>();
		anim.SetTrigger("unitFound");

		yield return new WaitForSeconds(0.2f);

		unitShine = false;
	}

	private Slot FindNearestEmptySlot(List<Slot> slots)
	{
		foreach (Slot slot in slots)
		{
			if (slot.payload == null)
			{
				return slot;
			}
		}

		return null;
	}

	public void ClearShopWindows()
	{
		// if the shop items are already being displayed
		if (shopWindows.Count != 0)
		{
			foreach (GameObject window in shopWindows)
			{
				if (window.GetComponent<Slot>().payload != null) Destroy(window.GetComponent<Slot>().payload);
				Destroy(window);
			} // delete them
			shopWindows.Clear(); // clear the list
			shopWindows.TrimExcess(); // wipe the memory
		}
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
				if (curShopItem.unitName.text == unit.name && curShopItem.unitName.text == battleManager.unitManager.unitStatsDatabase[i].name)
				{
					curUnitSlot.payload = battleManager.CreateUnitInstance(i, transform);
					curUnitSlot.payload.SetActive(false);
				}
			}
		}
	}

	// sets a single shop item at a certain location
	private void SetShopItem(Transform parent)
	{
		UnitInfo unitInfo;
		unitInfo = battleManager.unitManager.unitStatsDatabase[GetRandomUnitWeighted(defaultRarities)];

		for (int i = 0; i < shopUnits.Count; i++)
		{
			FindShopItem(); // find shopitem prefab and inits the curUnitInfo
			if (shopUnits[i].name == unitInfo.name)
			{
				curUnitInfo.curUnit = shopUnits[i];
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

		int roll = Random.Range(rarityOffsets[rarity], rarityOffsets[rarity] + unitRarityCount[rarity] - 1);
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