using System.Collections.Generic;
using UnityEngine;
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

	public GameObject gameOverUi;

	//[HideInInspector]
	public List<GameObject> allUnitPrfabs;



	public Actor playerActor;
	public Actor enemyActor;
	public List<BattleLane> lanes;

	public int amountOfBattlesBeforeShop = 2;
	public int amountOfBattlesCur;

	public bool resolved;

	private void Start()
	{
		Reset();
		LoadResources();

		StartNextBattle();

        playerActor.GetComponent<Health>().died.AddListener(playerDied);
        enemyActor.GetComponent<Health>().died.AddListener(enemyDied);
    }

	

	public void enemyDied()
	{
		Debug.Log("You won!");
		/*TODO 
		 * show win UI
		 * play enemy death animation
		 * play battle transition
		*/
		++amountOfBattlesCur;
		if(amountOfBattlesCur < amountOfBattlesBeforeShop)
        {
			StartNextBattle();
        }
        else
        {
			amountOfBattlesCur = 0;
			//load shop
        }
	}

	public void playerDied()
	{
		gameOverUi.SetActive(true);
	}

	public void ReloadScene()
	{
		SceneManager.LoadScene(0);
		Reset();
		StartNextBattle();
	}

	private void Reset()
	{
		gameOverUi.SetActive(false);
		resolved = false;
	}


	/// <summary>
	/// delete the old enemy units and then load a random enemy unit for each lane and assign targets and graves to all units
	/// </summary>
	void StartNextBattle()
    {
		//assigng target units to opposing units, and assign units to their graves
		foreach(BattleLane lane in lanes)
        {
			if(lane.enemyUnit) Destroy(lane.enemyUnit.gameObject);
			GameObject newUnitObj = Instantiate(GetRandomUnitObject(), lane.enemyUnitPosition);
			lane.enemyUnit = newUnitObj.GetComponent<UnitController>();
			lane.enemyUnit.parentActor = enemyActor;
			lane.enemyUnit.unitAttacker.targetUnit = lane.playerUnit;
			lane.enemyUnit.unitGrave = lane.enemyGrave;
			lane.playerUnit.unitAttacker.targetUnit = lane.enemyUnit;
			lane.playerUnit.unitGrave = lane.playerGrave;
			
		}
		enemyActor.health.Revive();
    }

	private void LoadResources()
    {
		Object[] loadedUnits;
		loadedUnits = Resources.LoadAll("Unit Prefabs");
		foreach( var lu  in loadedUnits)
        {
			allUnitPrfabs.Add((GameObject)lu);
        }
    }

	private void LoadPlayerUnitsIntoBattleField()
    {
		//todo
    }

	public GameObject GetRandomUnitObject()
    {
		int unitRoll = Random.Range(0, allUnitPrfabs.Count);
		return allUnitPrfabs[unitRoll];
    }

}
