using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;


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
	public List<GameObject> allUnitPrfabs;
	#endregion

	public List<Slot> playerBattleSlots = new List<Slot>();
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
	
	public bool isPaused { private set; get; }
	public UnityEvent pauseGame;
	public UnityEvent resumeGame;

	private bool inShop;
	
	private void Start()
	{
		mouseUtils = MouseUtils.singleton;
		
		Gold = 10;
		Lives = 3;
		LoadResources();

        playerActor.GetComponent<Health>().died.AddListener(PlayerDied);
        enemyActor.GetComponent<Health>().died.AddListener(EnemyDied);

		pauseGame.AddListener(PauseGame);
		resumeGame.AddListener(UnPauseGame);
    }

	void PauseGame()
    {
		isPaused = true;
		Time.timeScale = 0;
    }

	void UnPauseGame()
    {
		isPaused = false;
		Time.timeScale = 1;
	}
	

	public void EnemyDied()
	{
		battleWonEvent.Invoke();
		//Gold += battleReward;
		pauseGame.Invoke();
		/*TODO 
		 * show win UI
		 * play enemy death animation
		 * play battle transition
		*/
	}

	public void StartGame()
	{
		SceneManager.LoadScene("battle");
		LoadShop();
		mouseUtils.SetToDefaultCursor();
		inShop = true;
		HUD.SetActive(true);
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
		amountOfBattlesCur = 0;
		HideBattlefield();
		playerUnitsLoaded = false;
		shopTransitionEvent.Invoke();
		Gold = 10;
		foreach(Slot slot in playerBattleSlots)
        {
			slot.gameObject.SetActive(true);
        }
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
			lane.enemyUnit.unitAttacker.targetUnit = lane.playerUnit;
			lane.playerUnit.unitAttacker.targetUnit = lane.enemyUnit;
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
		foreach (Slot slot in playerBattleSlots)
		{
			slot.gameObject.SetActive(false);
		}
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
		int unitRoll = Random.Range(0, allUnitPrfabs.Count);
		return allUnitPrfabs[unitRoll];
    }
	private void LoadResources()
	{
		Object[] loadedUnits;
		loadedUnits = Resources.LoadAll("Unit Prefabs");
		foreach (var lu in loadedUnits)
		{
			allUnitPrfabs.Add((GameObject)lu);
		}
	}


}
