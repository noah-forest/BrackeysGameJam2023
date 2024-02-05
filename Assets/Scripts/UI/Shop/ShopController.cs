using System.Collections.Generic;
using System.Linq;
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

	private GameManager gameManager;
	private ShopAudio shopAudioPlayer;

	private GameObject shopItem;
	private GameObject shopWindow;

	private int unitIndex;
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
		PopulateShopUnits();

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
			}
		});

		Slot.anyDragStopped.AddListener(arg0 =>
		{
			if (arg0.payload != null && !draggedIntoShop)
			{
				shopAudioPlayer.PlayAudioClipOnce(shopAudioPlayer.audioClips[2]);
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
				draggedIntoShop = true;
				slot.payload = null;
				shopAudioPlayer.PlayAudioClipOnce(shopAudioPlayer.audioClips[3]);
				gameManager.Gold += 2;
			}
			return false;
		});
		#endregion
	}


	/// <summary>
	/// Populates the shop with units
	/// </summary>
	public void PopulateShopUnits()
	{
		firstRoll = true;

		// if the shop items are already being displayed
		if (shopWindows.Count != 0)
		{
			foreach (GameObject window in shopWindows) Destroy(window); // delete them
			shopWindows.Clear(); // clear the list
			shopWindows.TrimExcess(); // wipe the memory
		}

		// loop through the unit positions and place a new shop item there
		foreach (GameObject unitPos in shopItemPos)
		{
			SetShopItem(unitPos.transform);

			SetUnitInfo curShopItem = shopWindow.GetComponent<SetUnitInfo>();
			Slot unitSlot = shopWindow.GetComponent<Slot>();

			SetUnitPayload(unitSlot, curShopItem.unitName.text);
			curShopItem.prefab = unitSlot.payload;

			unitSlot.AddRetrievePrecheck((shopSlot, newSlot) =>
			{
				if (newSlot.payload != null)
				{
					return false;
				};

				if (gameManager.Gold >= curShopItem.unitCost)
				{
					curShopItem.purchased.SetActive(true);
					gameManager.Gold -= curShopItem.unitCost;
					return true;
				}

				draggedIntoNothing = true;
				return false;
			});

			shopWindows.Add(shopWindow);
		}
	}

	//loops through the folders containing the SOs
	//	grabs the corresponding prefab according to name
	//		and sets it to the payload

	private void SetUnitPayload(Slot curUnitSlot, string name)
	{
		foreach (Unit unit in shopUnits)
		{
			foreach (GameObject prefab in unitPrefabs)
			{
				if (name == unit.name && name == prefab.name)
				{
					curUnitSlot.payload = prefab;
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