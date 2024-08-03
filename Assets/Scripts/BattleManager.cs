using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.UI;
using UnityEngine;
using UnityEngine.UIElements;

public class BattleManager : MonoBehaviour
{
	#region singleton

	public static BattleManager singleton;

	private void Awake()
	{
		if (singleton)
		{
			Destroy(gameObject);
			return;
		}

		singleton = this;
		DontDestroyOnLoad(gameObject);
	}
	#endregion

	private GameManager gameManager;
	private MouseUtils mouseUtils;
	
	public UnitManager unitManager;
	public UnitPreview unitPreview;

	public GameObject battleField;
	public GameObject battleSlots;
	public GameObject unitMasterPrefab;

	public Actor playerActor;
	public Actor enemyActor;

	public List<BattleLane> lanes = new(); //manually set
	public List<Slot> playerBattleSlots = new(); //manually set
	public List<Slot> playerReserveSlots = new(); //manually set
	public List<GameObject> enemyUnits = new(); //auto populated

	public int interest;

	public int scaleToLvl2 = 3;
	public int scaleToLvl3 = 5;
	public int scaleToFinal = 7;

	private bool inShop;
	public bool firstTime;
	private bool playerUnitsLoaded;

	private void Start()
	{
		gameManager = GameManager.singleton;
		mouseUtils = MouseUtils.singleton;

		playerActor.GetComponent<Health>().died.AddListener(PlayerDied);
		enemyActor.GetComponent<Health>().died.AddListener(EnemyDied);
	}

	public void EnemyDied()
	{
		++gameManager.BattlesWon;

		gameManager.battleWonEvent.Invoke();
		gameManager.battleEndedEvent.Invoke();
		gameManager.pauseGame.Invoke();
	}

	public void StartGame()
	{
		gameManager.startBattle.Invoke();
		gameManager.uiManager.ResetHearts();
		firstTime = true;
		gameManager.Lives = 3;
		gameManager.BattlesWon = 0;
		inShop = true;
		gameManager.MusicPlayer.Play();
		battleField.SetActive(true);

		foreach (Slot slot in playerBattleSlots)
		{
			Destroy(slot.payload);
			slot.payload = null;
		}

		foreach (Slot slot in playerReserveSlots)
		{
			Destroy(slot.payload);
			slot.payload = null;
		}
	}

	public bool CheckForReserveSlot()
	{
		if (playerReserveSlots.Count <= 0) return false;
		else return true;
	}

	public void PlayerDied()
	{
		--gameManager.Lives;
		if (gameManager.Lives > 0)
		{
			gameManager.battleLostEvent.Invoke();
		}
		else
		{
			gameManager.gameOverEvent.Invoke();
		}
		gameManager.battleEndedEvent.Invoke();
		gameManager.pauseGame.Invoke();
	}

	public void NextBattleButton()
	{
		if (inShop)
		{
			StartNextBattle();
		}
		else
		{
			LoadShop();
		}
	}

	public void GainBattleReward()
	{
		gameManager.Cash += gameManager.settings.battleReward;
	}

	public int GainInterest()
	{
		int interestThreshold = 5;
		if (gameManager.Cash >= interestThreshold)
		{
			interest = gameManager.Cash / interestThreshold;
		}

		return interest;
	}

	/// <summary>
	/// 1 is shop atm 8/23, if this stops working, the shop scene probably changed in the build settings
	/// </summary>
	public void LoadShop()
	{
		if (firstTime)
		{
			gameManager.Cash = gameManager.settings.startingGold;
		}

		if (gameManager.debugMenu.gainInterest && gameManager.debugMenu.growingMoney && !firstTime)
		{
			gameManager.Cash += GainInterest();
			GainBattleReward();
		}
		else if (!gameManager.debugMenu.gainInterest && gameManager.debugMenu.growingMoney && !firstTime)
		{
			GainBattleReward();
		}
		else
		{
			gameManager.Cash = gameManager.settings.startingGold;
		}

		firstTime = false;
		gameManager.resumeGame.Invoke();
		HideBattlefield();
		CreateEnemyTeam();
		playerUnitsLoaded = false;
		gameManager.loadShopEvent.Invoke();
		mouseUtils.SetToDefaultCursor();
		mouseUtils.FindButtonsInScene();
		battleSlots.SetActive(true);
	}

	public void ShopTransition()
	{
		gameManager.startShopTransition.Invoke();
	}

	public void BattleTransition()
	{
		gameManager.startBattleTransition.Invoke();
	}

	void StartNextBattle()
	{
		mouseUtils.SetToDefaultCursor();
		gameManager.battleStartedEvent.Invoke();
		gameManager.resumeGame.Invoke();
		ShowBattlfield();
		StartCoroutine(LoadUnits()); //dont let this shit get into final build please -> <- (it will) 
	}

	private IEnumerator LoadUnits()
	{
		yield return new WaitForSeconds(0.2f);

		LoadEnemyTeamIntoBattle();
		if (!playerUnitsLoaded) LoadPlayerUnitsIntoBattle();
		AssignUnitTargets();
	}

	public void HideBattlefield()
	{
		ClearBattlefield();
		battleField.SetActive(false);
	}

	public void ShowBattlfield()
	{
		ClearBattlefield();
		battleField.SetActive(true);
	}
	private void AssignUnitTargets()
	{
		foreach (BattleLane lane in lanes)
		{
			if (lane.enemyUnitController) lane.enemyUnitController.unitAttacker.targetUnit = lane.playerUnit;
			if (lane.playerUnit) lane.playerUnit.unitAttacker.targetUnit = lane.enemyUnitController;
		}
	}

	public void CreateEnemyTeam()
	{
		if (enemyUnits.Count != 0)
		{
			foreach (GameObject unit in enemyUnits)
			{
				Destroy(unit);
			}
			enemyUnits.Clear();
			enemyUnits.TrimExcess();
		}

		foreach (BattleLane lane in lanes)
		{
			if (lane.enemyUnit) Destroy(lane.enemyUnit);
			lane.enemyUnit = InstantiateRandomUnit(lane.enemyUnitPosition);
			enemyUnits.Add(lane.enemyUnit);
		}

		//populates the unitPreview with the generated units
		unitPreview.FillUnitPos(enemyUnits);
	}

	/// <summary>
	/// deletes the old enemy units if there are any and then loads a random enemy unit for each lane and assign graves to them
	/// </summary>
	private void LoadEnemyTeamIntoBattle()
	{
		foreach (BattleLane lane in lanes)      //assigng target units to opposing units, and assign units to their graves
		{
			lane.enemyUnitController = lane.enemyUnit.GetComponent<UnitController>();
			lane.enemyUnitController.parentActor = enemyActor;
			lane.enemyUnitController.unitGrave = lane.enemyGrave;
			lane.enemyUnit.SetActive(true);

			lane.enemyUnitController.InitCombat();
		}
		enemyActor.health.Revive();
	}

	/// <summary>
	/// spawns the units in the players battleslots into the battlefiled and assign graves to them
	/// </summary>
	private void LoadPlayerUnitsIntoBattle()
	{
		playerUnitsLoaded = true;
		//hide battle slots;
		battleSlots.SetActive(false);
		//load units
		for (int unitIdx = 0; unitIdx < lanes.Count; unitIdx++)
		{
			if (playerBattleSlots[unitIdx].payload)
			{
				GameObject newUnitObj = playerBattleSlots[unitIdx].payload;
				newUnitObj.transform.parent = lanes[unitIdx].playerUnitPosition;
				newUnitObj.SetActive(true);
				newUnitObj.transform.position = lanes[unitIdx].playerUnitPosition.position;
				newUnitObj.transform.localPosition = Vector3.zero;

				//flip players units so that the attack anim plays the correct way
				newUnitObj.transform.localScale = new Vector3(-1, 1, 1);
				//then flip the sprites so that they aren't facing the wrong way
				newUnitObj.GetComponentInChildren<SpriteRenderer>().flipX = true;

				//since we flip the unit, we need to manually flip the shadow on the health bar to be correct
				UnitHealthBar healthBar = newUnitObj.GetComponentInChildren<UnitHealthBar>();
				healthBar.shadowFlipped.SetActive(true);
				healthBar.shadowDefault.SetActive(false);

				lanes[unitIdx].playerUnit = newUnitObj.GetComponent<UnitController>();
				lanes[unitIdx].playerUnit.parentActor = playerActor;
				lanes[unitIdx].playerUnit.unitGrave = lanes[unitIdx].playerGrave;

				lanes[unitIdx].playerUnit.InitCombat();
			}
			else
			{
				lanes[unitIdx].playerUnit = null;
			}
		}
	}

	private void ClearBattlefield()
	{
		for (int i = 0; i < lanes.Count; i++)
		{
			if (lanes[i].enemyUnitController) Destroy(lanes[i].enemyUnitController.gameObject);
			if (lanes[i].playerUnit)
			{
				lanes[i].playerUnit.Respawn();
				lanes[i].playerUnit.gameObject.SetActive(false);
			}
		}
		enemyActor.health.Revive();
		playerActor.health.Revive();
		playerUnitsLoaded = false;
	}

	public GameObject InstantiateRandomUnit(Transform parent)
	{
		int unitRoll = Random.Range(0, unitManager.unitStatsDatabase.Count);

		int unitLevelRoll = 0;

		//after certain amount of battlesWon, start scaling enemy units

		if (gameManager.BattlesWon >= scaleToLvl2)
		{
			unitLevelRoll = Random.Range(0, 2);

			if (gameManager.BattlesWon >= scaleToLvl3)
			{
				unitLevelRoll = Random.Range(1, 3);
			}

			if(gameManager.BattlesWon >= scaleToFinal)
			{
				unitLevelRoll = 2;
			}
		}

		if (unitLevelRoll == 1)
		{
			return CreateUnitInstance(unitRoll, parent, Experience.ExpToLevel2);
		}
		else if (unitLevelRoll == 2)
		{
			return CreateUnitInstance(unitRoll, parent, Experience.ExpToLevel3);
		} else
		{
			return CreateUnitInstance(unitRoll, parent);
		}
	}

	public GameObject CreateUnitInstance(int unitIndex, Transform parent, int unitLevel = 0)
	{
		GameObject newUnit = Instantiate(unitMasterPrefab, parent);
		UnitMasterComponent unitMaster = newUnit.GetComponent<UnitMasterComponent>();
		unitMaster.unitStats.InitUnit(unitManager.unitStatsDatabase[unitIndex]);

		//artifically level units when given a unitLevel param
		//this is used for enemy unit scaling, but could have other applications
		if (unitLevel == 2)
		{
			unitMaster.unitExperience.AddExp(unitLevel);
		}
		else if (unitLevel == 3)
		{
			unitMaster.unitExperience.AddExp(Experience.ExpToLevel2);
			unitMaster.unitExperience.AddExp(unitLevel);
		}

		unitMaster.unitStats.Rarity = unitManager.unitStatsDatabase[unitIndex].rarity;
		unitMaster.unitStats.description = unitManager.unitStatsDatabase[unitIndex].description;
		newUnit.name = unitManager.unitStatsDatabase[unitIndex].name;

		unitMaster.unitSprite.sprite = unitManager.unitStatsDatabase[unitIndex].unitSprite;

		return newUnit;
	}
}
