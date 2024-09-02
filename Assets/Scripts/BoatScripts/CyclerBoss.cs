﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;

namespace Assets.Scripts.BoatScripts
{
    [CreateAssetMenu(fileName = "New CyclerEncounter", menuName = "Boat Game/Boss Encounter/Cycler Boss")]
    public class CyclerBoss : BossEncounterSO
    {

        [SerializeField]
        List<Team> CycleTeams = new List<Team>();

        Dictionary<BattleLane, int> LaneToTeam = new Dictionary<BattleLane, int>();


        public override void PreBattle()
        {
            base.PreBattle();
            LaneToTeam.Clear();
            foreach (BattleLane lane in gameManager.battleManager.lanes)
            {
                LaneToTeam.Add(lane, 0);
            }
        }

        public override int GetUnitID(int LaneIdx)
        {
            return CycleTeams[
                LaneToTeam[
                    gameManager.battleManager.lanes[LaneIdx]
                ]
            ].units[LaneIdx].UnitID;
        }
        public override int GetUnitLvl(int LaneIdx)
        {
            return CycleTeams[LaneToTeam[gameManager.battleManager.lanes[LaneIdx]]].units[LaneIdx].level;
        }

        public override void BattleStart()
        {
            base.BattleStart();
            for (int i = 0; i < gameManager.battleManager.enemyUnits.Count; i++)
            {
                GameObject unitObj = gameManager.battleManager.enemyUnits[i];
                UnitController uc = unitObj.GetComponent<UnitController>();
                uc?.unitRespawnEvent.AddListener(OnBossUnitDied);
            }
        }

        void OnBossUnitDied(UnitController unit)
        {
            //THIS is super jank and prone to breaking if the ui structure is changed.
            BattleLane lane = unit.gameObject.transform.parent.parent.GetComponent<BattleLane>();
            Debug.Log($"{unit.gameObject.name} -> {unit.gameObject.transform.parent.name} - > {unit.gameObject.transform.parent.parent.name}");

            if (lane)
            {
                ++LaneToTeam[lane];
                if (LaneToTeam[lane] >= CycleTeams.Count)
                {
                    LaneToTeam[lane] = 0;
                }
                GameObject newUnit = gameManager.battleManager.InstatiateBossEncounterUnit(unit.transform.parent, lane.LaneID);
                lane.enemyUnit = newUnit;
                lane.enemyUnitController = lane.enemyUnit.GetComponent<UnitController>();
                lane.enemyUnitController.unitRespawnEvent.AddListener(OnBossUnitDied);
                lane.enemyUnitController.parentActor = enemyActor;
                lane.enemyUnitController.unitGrave = lane.enemyGrave;
                lane.enemyUnit.SetActive(true);
                lane.enemyUnitController.health.OwnerHealth = enemyActor.health;
                lane.enemyUnitController.InitCombat();
                lane.enemyUnitController.unitAttacker.target = lane.playerUnit ? lane.playerUnit.health : playerActor.health;
                if (lane.playerUnit) lane.playerUnit.unitAttacker.target = lane.enemyUnitController ? lane.enemyUnitController.health : enemyActor.health;
                Destroy(unit.gameObject);
            }
            else 
            {
                Debug.Log("[BOSS ENCOUNTER] Cycler Boss relys on specific lane structure. If this has been changed, the boss will not work correctly.");
            }
        }
    }
}
