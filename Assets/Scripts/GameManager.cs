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

    #region Battle Events
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
	public int maxLives;

	[HideInInspector]
	public List<GameObject> allUnitPrefabs;
	#endregion

	public List<Slot> playerBattleSlots = new List<Slot>();
	public GameObject battleSlots;
	bool playerUnitsLoaded;

	public GameObject HUD;

	public GameObject wonParticles;
	
	[SerializeField] GameObject battleField;

	[SerializeField] AudioSource MusicPlayer;

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

	private bool inShop;
	
	private void Start()
	{
		mouseUtils = MouseUtils.singleton;
		
		Gold = 150;
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
		} else
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
		maxLives = Lives;
		Gold = 150;
		inShop = true;
		MusicPlayer.Play();
		battleField.SetActive(true);
		foreach (Slot slot in playerBattleSlots)
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
		Gold = 12;
		if (inShop)
		{
			StartNextBattle();
			++amountOfBattlesCur;
		}
		else if (amountOfBattlesCur < amountOfBattlesBeforeShop)
		{
			StartNextBattle();
			++amountOfBattlesCur;
		} else
		{
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
		resumeGame.Invoke();
		battleStartedEvent.Invoke();
		ShowBattlfield();
		StartCoroutine(LoadUnits()); //dont let this shit get into final build please -> <-
	}


	//definitely a band-aid fix please fix this later i beg of you
	private IEnumerator LoadUnits()
	{
		yield return new WaitForSeconds(0.2f);

		LoadRandomEnemyTeamIntoBattle();
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
		}
	}

	private void ClearBattlefield()
    {
		for(int i = 0; i < lanes.Count; i++)
		{
			if (lanes[i].enemyUnit) Destroy(lanes[i].enemyUnit.gameObject);
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
