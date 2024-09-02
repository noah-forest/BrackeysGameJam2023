using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New StandardEncounter", menuName = "Boat Game/Boss Encounter/Set Team")]
public class BossEncounterSO : ScriptableObject
{
    [Serializable]
    public struct TeamUnit
    {
        [Range(1,3)]
        public int level;
        [SerializeField] int unitID;
        public int UnitID { get
            {
                if (useRandomID)
                {
                    return UnityEngine.Random.Range(0, UnitManager.singleton.unitStatsDatabase.Count);
                }
                return unitID;
            } 
        }
        public bool useRandomID;
    }
    [Serializable]
    public class Team
    {
        public TeamUnit[] units = new TeamUnit[3];
    }

    protected GameManager gameManager;
    protected Actor enemyActor;
    protected Actor playerActor;
    public Team bossTeam;

    public virtual int GetUnitID(int LaneIdx)
    {
        return bossTeam.units[LaneIdx].UnitID;
    }
    public virtual int GetUnitLvl(int LaneIdx)
    {
        return bossTeam.units[LaneIdx].level;
    }

    public virtual void FixedUpdateLogic(float fixedDeltaTime) { }
    public virtual void UpdateLogic(float deltaTime) { }
    public virtual void PreBattle() 
    {
        gameManager = GameManager.singleton;
        enemyActor = gameManager.battleManager.enemyActor;
        playerActor = gameManager.battleManager.playerActor;
    }
    public virtual void BattleStart() { }
    public virtual void PostBattle() { }


}
