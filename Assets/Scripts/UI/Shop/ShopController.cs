using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

public class ShopController : MonoBehaviour
{
	// this is for the UI
	public List<GameObject> shopItemPos = new();
	private List<GameObject> shopWindows = new();

	// lists to populate from resources
	private List<Unit> shopUnits = new();
	private List<GameObject> unitPrefabs = new();

	public GameObject sellWindow;
	public TextMeshProUGUI sellWindowPrice;

	private GameManager gameManager;
	private ShopAudio shopAudioPlayer;

	private GameObject shopItem;
	private GameObject shopWindow;

	private int unitIndex;
	private int sellCost;
	public int refreshCost;

	private SetUnitInfo curUnitInfo;

	private bool draggedIntoShop;
	private bool draggedIntoNothing;

	public bool firstRoll;          // this is to check if the shop has been initially rolled

	private void Start()
	{
		gameManager = GameManager.singleton;
		shopAudioPlayer = GetComponent<ShopAudio>();

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

			if (arg0.payload != null)
			{
				shopAudioPlayer.PlayAudioClipOnce(shopAudioPlayer.audioClips[1]);
				UnitStats sellInfo = arg0.payload.GetComponent<UnitStats>();
				sellWindowPrice.text = $"{sellInfo.sellValue}";
				//Debug.Log("playing pick up sound");
			}
		});

		Slot.anyDragStopped.AddListener(arg0 =>
		{
			if (arg0.payload != null && !draggedIntoShop)
			{
				shopAudioPlayer.PlayAudioClipOnce(shopAudioPlayer.audioClips[2]);
				//Debug.Log("playing drop sound");
			}

			if (arg0.CompareTag("BattleSlot") || arg0.CompareTag("ReserveSlot"))
			{
				sellWindow.SetActive(false);
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
				gameManager.Gold += (int)sellInfo.sellValue;
				Destroy(slot.payload);
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

			SetUnitPayload(unitSlot, curShopItem);

			UnitStats sellInfo = unitSlot.payload.GetComponent<UnitStats>();
			sellInfo.sellValue = (int)(curShopItem.unitCost * 0.75);

			unitSlot.AddRetrievePrecheck((shopSlot, newSlot) =>
			{
				if (newSlot.payload != null && newSlot.payload.name != unitSlot.payload.name || newSlot.gameObject.name.Contains("Shop"))
				{
					return false;
				};

				if (gameManager.Gold >= curShopItem.unitCost)
				{
					if(newSlot.payload != null)
					{
						Experience unitExp = newSlot.payload.GetComponent<Experience>();
						if (unitExp && unitExp.curLevel == Experience.MaxLevel) return false;
					}

					curShopItem.purchased.SetActive(true);
					gameManager.Gold -= curShopItem.unitCost;
					shopAudioPlayer.PlayAudioClipOnce(shopAudioPlayer.audioClips[3]);
					return true;
				}

				draggedIntoNothing = true;
				return false;
			});

			shopWindows.Add(shopWindow);
		}

		 firstRoll = true;
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
			foreach (GameObject prefab in unitPrefabs)
			{
				if (curShopItem.unitName.text == unit.name && curShopItem.unitName.text == prefab.name)
				{
					curUnitSlot.payload = Instantiate(prefab, transform);
					curUnitSlot.payload.SetActive(false);
				}
			}
		}
	}

	// randomly sets a single shop item at a certain location
	private void SetShopItem(Transform parent)
	{
		for (int i = 0; i < shopUnits.Count; i++)
		{
			FindShopItem(); // find shopitem prefab and inits the curUnitInfo 

			unitIndex = Random.Range(0, shopUnits.Count);
			curUnitInfo.curUnit = shopUnits[unitIndex]; // sets the current unit to a random one in the list of units
		}

		shopWindow = Instantiate(shopItem, parent); // creates the window object (1 of 4)
	}

	private void FindShopItem()
	{
		shopItem = Resources.Load("Prefabs/UI/Shop/unitShopItem") as GameObject;
		if (shopItem != null)
		{
			curUnitInfo = shopItem.GetComponent<SetUnitInfo>();
		}
	}

	// populates the lists used for units and unit prefabs
	private void LoadResources()
	{
		shopUnits = Resources.LoadAll<Unit>("SOs/Units").ToList();

		Object[] loadedUnits = Resources.LoadAll("Prefabs/Units");
		foreach (var lu in loadedUnits)
		{
			unitPrefabs.Add((GameObject)lu);
		}
	}
}