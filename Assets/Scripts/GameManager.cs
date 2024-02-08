using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;
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

    #region BattleEvents
    [HideInInspector]
	public UnityEvent goldChangedEvent;
	[HideInInspector]
	public UnityEvent livesChangedEvent;
	[HideInInspector]
	public UnityEvent battleWonEvent;
	[HideInInspector]
	public UnityEvent battleLostEvent;
	[HideInInspector]
	public UnityEvent gameOverEvent;
	[HideInInspector]
	public UnityEvent battleStartedEvent;
	[HideInInspector]
	public UnityEvent shopTransitionEvent;
    #endregion

    #region Lives and Gold properties
    /// <summary>
    ///dont use this variable, use Gold
    /// </summary>
    private int _internalGold;
	public int Gold
    {
		get => _internalGold;
		set
        {
			_internalGold = value;
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
	[HideInInspector]
	public List<GameObject> allUnitPrefabs;
	#endregion

	public List<Slot> playerBattleSlots = new List<Slot>();
	public GameObject battleSlots;
	bool playerUnitsLoaded;

	public GameObject HUD;
	
	[SerializeField] GameObject battleField;

	public Actor playerActor;
	public Actor enemyActor;
	public List<BattleLane> lanes;

	public int battleReward = 3;
	public int amountOfBattlesBeforeShop = 2;
	public int amountOfBattlesCur = 0;

	[HideInInspector] public MouseUtils mouseUtils;
	
	public bool gameIsPaused { private set; get; }
	public bool openThePauseMenuPleaseGoodSir;

	[HideInInspector]
	public UnityEvent pauseGame;
	[HideInInspector]
	public UnityEvent resumeGame;
	[HideInInspector]
	public UnityEvent startGame;
	[HideInInspector]
	public UnityEvent startBattle;

	public UnityEvent loadUI;

	private bool inShop;
	
	private void Start()
	{
		mouseUtils = MouseUtils.singleton;
		
		Gold = 15;
		Lives = 3;
		LoadResources();

        playerActor.GetComponent<Health>().died.AddListener(PlayerDied);
        enemyActor.GetComponent<Health>().died.AddListener(EnemyDied);

		pauseGame.AddListener(PauseGame);
		resumeGame.AddListener(UnPauseGame);
    }

    private void Update()
    {
		if (Input.GetKeyDown(KeyCode.Escape))
		{
			TogglePauseMenu();
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
				openThePauseMenuPleaseGoodSir = false;
				resumeGame.Invoke();
			}
			else
			{
				openThePauseMenuPleaseGoodSir = true;
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
		battleWonEvent.Invoke();
		pauseGame.Invoke();
	}

	public void StartGame()
	{
		startBattle.Invoke();
		Lives = 3;
		Gold = 15;
		inShop = true;
		foreach(Slot slot in playerBattleSlots)
        {
			slot.payload = null;
        }
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
		pauseGame.Invoke();
	}
	
	public void NextBattleButton()
	{
		Gold = 10;
		if (inShop)
		{
			// play transition animation
			StartNextBattle();
			++amountOfBattlesCur;
		}
		else if (amountOfBattlesCur < amountOfBattlesBeforeShop)
		{
			// play transition animation
			StartNextBattle();
			++amountOfBattlesCur;
		}
		else
		{
			Debug.Log("loading shop");
			LoadShop();
		}
    }

	/// <summary>
	/// 1 is shop atm 8/23, if this stops working, the shop scene probably changed in the build settings
	/// </summary>
	public void LoadShop()
    {
	    resumeGame.Invoke();
		amountOfBattlesCur = 0;
		HideBattlefield();
		playerUnitsLoaded = false;
		shopTransitionEvent.Invoke();
		mouseUtils.SetToDefaultCursor();
		mouseUtils.FindButtonsInScene();
		battleSlots.SetActive(true);
    }

	void StartNextBattle()
    {
	    mouseUtils.SetToDefaultCursor();
		resumeGame.Invoke();
		battleStartedEvent.Invoke();
		ShowBattlfield();
		if(!playerUnitsLoaded) LoadPlayerUnitsIntoBattle();
		LoadRandomEnemyTeamIntoBattle();
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
	private void AssignUnitTargets(){
		foreach(BattleLane lane in lanes)
        {
			if(lane.enemyUnit) lane.enemyUnit.unitAttacker.targetUnit = lane.playerUnit;
			if(lane.playerUnit) lane.playerUnit.unitAttacker.targetUnit = lane.enemyUnit;
		}
    }

	/// <summary>
	/// deletes the old enemy units if there are any and then loads a random enemy unit for each lane and assign graves to them
	/// </summary>
	private void LoadRandomEnemyTeamIntoBattle()
    {
		foreach (BattleLane lane in lanes)      //assigng target units to opposing units, and assign units to their graves
		{
			if (lane.enemyUnit) Destroy(lane.enemyUnit.gameObject);
			GameObject newUnitObj = Instantiate(GetRandomUnitObject(), lane.enemyUnitPosition);
			lane.enemyUnit = newUnitObj.GetComponent<UnitController>();
			lane.enemyUnit.parentActor = enemyActor;
			lane.enemyUnit.unitGrave = lane.enemyGrave;
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
		for (int unitIdx=0; unitIdx < lanes.Count; unitIdx++)
        {
            if (playerBattleSlots[unitIdx].payload)
            {
				GameObject newUnitObj = Instantiate(playerBattleSlots[unitIdx].payload, lanes[unitIdx].playerUnitPosition.position, lanes[unitIdx].playerUnitPosition.rotation);
				newUnitObj.transform.localScale = new Vector3(-1, 1, 1);
				newUnitObj.GetComponentInChildren<SpriteRenderer>().flipX = true;
				lanes[unitIdx].playerUnit = newUnitObj.GetComponent<UnitController>();
				lanes[unitIdx].playerUnit.parentActor = playerActor;
				lanes[unitIdx].playerUnit.unitGrave = lanes[unitIdx].playerGrave;
			}

		}

	}

	private void ClearBattlefield()
    {
		foreach (BattleLane lane in lanes)
		{
			if (lane.enemyUnit) Destroy(lane.enemyUnit.gameObject);
			if (lane.playerUnit) Destroy(lane.playerUnit.gameObject);
		}
		enemyActor.health.Revive();
		playerActor.health.Revive();
		playerUnitsLoaded = false;
	}

	public GameObject GetRandomUnitObject()
    {
		int unitRoll = Random.Range(0, allUnitPrefabs.Count);
		return allUnitPrefabs[unitRoll];
    }
	private void LoadResources()
	{
		Object[] loadedUnits;
		loadedUnits = Resources.LoadAll("Prefabs/Units");
		foreach (var lu in loadedUnits)
		{
			allUnitPrefabs.Add((GameObject)lu);
		}
	}


}
