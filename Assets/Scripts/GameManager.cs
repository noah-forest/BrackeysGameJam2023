using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;


public class GameManager : MonoBehaviour
{
	#region singleton

	public static GameManager singleton;

	private void Awake()
	{
		if (singleton)
		{
			Destroy(this.gameObject);
			return;
		}

		singleton = this;
		DontDestroyOnLoad(this.gameObject);
	}
	#endregion

	#region Battle Events
	[HideInInspector]
	public UnityEvent goldChangedEvent;
	[HideInInspector]
	public UnityEvent livesChangedEvent;
	[HideInInspector]
	public UnityEvent battleScoreEvent;
	[HideInInspector]
	public UnityEvent battleWonEvent;
	[HideInInspector]
	public UnityEvent battleLostEvent;
	[HideInInspector]
	public UnityEvent gameOverEvent;
	[HideInInspector]
	public UnityEvent battleStartedEvent;
	[HideInInspector]
	public UnityEvent combatBeganEvent;
	[HideInInspector]
	public UnityEvent battleEndedEvent;
	[HideInInspector]
	public UnityEvent loadShopEvent;
	#endregion

	#region Transition Events
	[HideInInspector]
	public UnityEvent startGame;
	[HideInInspector]
	public UnityEvent startBattle;
	[HideInInspector]
	public UnityEvent startBattleTransition;
	[HideInInspector]
	public UnityEvent startShopTransition;

	[HideInInspector]
	public UnityEvent loadUI;

	#endregion

	#region Shop Events
	[HideInInspector]
	public UnityEvent shopRefreshed = new();
	[HideInInspector]
	public UnityEvent unitPurchased = new();
	[HideInInspector]
	public UnityEvent unitSold = new();
	[HideInInspector]
	public UnityEvent enemyUnitsCreated = new();
	[HideInInspector]
	public UnityEvent previewRolled = new();
	[HideInInspector]
	public UnityEvent unitRevealed = new();
	#endregion

	#region Unit Events
	[HideInInspector]
	public UnityEvent unitExpAdded;
	[HideInInspector]
	public UnityEvent unitLevelUp;
	#endregion

	[HideInInspector] public UnityEvent unitAddedToSlot = new();

	#region Getters and Setters
	/// <summary>
	///dont use this variable, use Cash
	/// </summary>
	private int _internalCash;
	public int Cash
	{
		get => _internalCash;
		set
		{
			_internalCash = value;
			goldChangedEvent.Invoke();
		}
	}

	/// <summary>
	///dont use this variable, use Lives
	/// </summary>
	private int _internalLives;
	public int Lives
	{
		get => _internalLives;
		set
		{
			_internalLives = value;
			livesChangedEvent.Invoke();
		}
	}

	/// <summary>
	///dont use this variable, use BattlesWon
	/// </summary>
	private int _internalBattlesWon;
	public int BattlesWon
	{
		get => _internalBattlesWon;
		set
		{
			_internalBattlesWon = value;
			battleScoreEvent.Invoke();
		}
	}
	#endregion

	[Space(10)]

	public DebugMenu debugMenu;
	public Settings settings;
	public GameObject settingsMenu;

	[Space(10)]

	public UnitManager unitManager;
	public UIManager uiManager;

	[Header("everything else")]

	public List<Slot> playerBattleSlots = new List<Slot>();
	public List<Slot> playerReserveSlots = new();
	public GameObject battleSlots;
	bool playerUnitsLoaded;

	public GameObject HUD;

	public GameObject wonParticles;

	[SerializeField] GameObject battleField;

	public AudioSource MusicPlayer;
	public AudioMixer audioMixer;

	public Actor playerActor;
	public Actor enemyActor;
	public List<BattleLane> lanes;

	public List<GameObject> enemyUnits;

	[HideInInspector] public MouseUtils mouseUtils;

	public bool gameIsPaused { private set; get; }
	public bool openPauseMenu;
	private bool firstTime;

	[HideInInspector]
	public UnityEvent pauseGame;
	[HideInInspector]
	public UnityEvent resumeGame;

	private bool inShop;

	public GameObject unitMasterPrefab;

	public UnitPreview unitPreview;
	public int revealCost = 5;

	public int interest;

	private int EnemyUnitLvl2 = 3;
	private int EnemyUnitLvl3 = 6;

	private void Start()
	{
		mouseUtils = MouseUtils.singleton;

		playerActor.GetComponent<Health>().died.AddListener(PlayerDied);
		enemyActor.GetComponent<Health>().died.AddListener(EnemyDied);

		pauseGame.AddListener(PauseGame);
		resumeGame.AddListener(UnPauseGame);

		EnemyUnitLvl2 = 1;

		firstTime = true;
	}

	private void Update()
	{
		if (SceneManager.GetActiveScene().name == "MainMenu") return;
		if (Input.GetKeyDown(KeyCode.Escape))
		{
			TogglePauseMenu();

			if (settingsMenu.activeInHierarchy)
			{
				OpenSideMenu menu = settingsMenu.GetComponent<OpenSideMenu>();
				menu.OnClick();
			}
		}
	}

	public void BackToMainMenu()
	{
		resumeGame.Invoke();
		startGame.Invoke();
	}

	public void TogglePauseMenu()
	{
		if (gameIsPaused)
		{
			openPauseMenu = false;
			mouseUtils.SetToDefaultCursor();
			resumeGame.Invoke();
		}
		else
		{
			openPauseMenu = true;
			mouseUtils.SetToDefaultCursor();
			pauseGame.Invoke();
		}
	}

	public void PauseGame()
	{
		gameIsPaused = true;
		Time.timeScale = 0;
	}

	public void UnPauseGame()
	{
		gameIsPaused = false;
		Time.timeScale = 1;
	}

	public void EnemyDied()
	{
		++BattlesWon;
		Debug.Log($"battles won: {BattlesWon}");
		battleWonEvent.Invoke();
		battleEndedEvent.Invoke();
		pauseGame.Invoke();
	}

	public void StartGame()
	{
		startBattle.Invoke();
		uiManager.ResetHearts();
		firstTime = true;
		Lives = 3;
		inShop = true;
		MusicPlayer.Play();
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
		--Lives;
		if (Lives > 0)
		{
			battleLostEvent.Invoke();
		}
		else
		{
			gameOverEvent.Invoke();
		}
		battleEndedEvent.Invoke();
		pauseGame.Invoke();
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
		Cash += settings.battleReward;
	}

	public int GainInterest()
	{
		int interestThreshold = 5;
		if (Cash >= interestThreshold)
		{
			interest = Cash / interestThreshold;
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
			Cash = settings.startingGold;
		}

		if (debugMenu.gainInterest && debugMenu.growingMoney && !firstTime)
		{
			Cash += GainInterest();
			GainBattleReward();
		}
		else if (!debugMenu.gainInterest && debugMenu.growingMoney && !firstTime)
		{
			GainBattleReward();
		}
		else
		{
			Cash = settings.startingGold;
		}

		firstTime = false;
		resumeGame.Invoke();
		HideBattlefield();
		CreateEnemyTeam();
		playerUnitsLoaded = false;
		loadShopEvent.Invoke();
		mouseUtils.SetToDefaultCursor();
		mouseUtils.FindButtonsInScene();
		battleSlots.SetActive(true);
	}

	public void ShopTransition()
	{
		startShopTransition.Invoke();
	}

	public void BattleTransition()
	{
		startBattleTransition.Invoke();
	}

	void StartNextBattle()
	{
		mouseUtils.SetToDefaultCursor();
		battleStartedEvent.Invoke();
		resumeGame.Invoke();
		ShowBattlfield();
		StartCoroutine(LoadUnits()); //dont let this shit get into final build please -> <-
	}


	//definitely a band-aid fix please fix this later i beg of you
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
		if(enemyUnits.Count != 0)
		{
			foreach (GameObject unit in enemyUnits)
			{
				Destroy(unit);
			}
			enemyUnits.Clear();
			enemyUnits.TrimExcess();
		}

		foreach(BattleLane lane in lanes)
		{
			if (lane.enemyUnit) Destroy(lane.enemyUnit);
			lane.enemyUnit = InstantiateRandomUnit(lane.enemyUnitPosition);
			enemyUnits.Add(lane.enemyUnit);
		}

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
				newUnitObj.transform.SetPositionAndRotation(lanes[unitIdx].playerUnitPosition.position, lanes[unitIdx].playerUnitPosition.rotation);
				newUnitObj.transform.localScale = new Vector3(-1, 1, 1);
				newUnitObj.GetComponentInChildren<SpriteRenderer>().flipX = true;
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

		if (BattlesWon >= EnemyUnitLvl2)
		{
			int unitLevelRoll = Random.Range(0, 2);

			if(BattlesWon >= EnemyUnitLvl3)
			{
				unitLevelRoll = Random.Range(0, 3);
			}

			if(unitLevelRoll == 1)
			{
				return CreateUnitInstance(unitRoll, parent, Experience.ExpToLevel2);
			} else if(unitLevelRoll == 2)
			{
				return CreateUnitInstance(unitRoll, parent, Experience.ExpToLevel3);
			}
		}

		return CreateUnitInstance(unitRoll, parent);
	}

	public GameObject CreateUnitInstance(int unitIndex, Transform parent, int unitLevel = 0)
	{
		GameObject newUnit = Instantiate(unitMasterPrefab, parent);
		UnitMasterComponent unitMaster = newUnit.GetComponent<UnitMasterComponent>();
		unitMaster.unitStats.InitUnit(unitManager.unitStatsDatabase[unitIndex]);

		if(unitLevel == 2)
		{
			unitMaster.unitExperience.AddExp(unitLevel);
		} else if(unitLevel == 3)
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
