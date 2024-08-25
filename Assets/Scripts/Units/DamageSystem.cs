using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

namespace Assets.Scripts.Units
{
    /// <summary>
    /// struct containing information about an instance of damage thats trying to be dealt
    /// </summary>
    /// 


    public class DamageSystem : MonoBehaviour
    {
        [SerializeField] bool logDamageReports;
        #region singleton
        public static DamageSystem singleton;

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

        #endregion singleton

        public UnityEvent<DamageReport> OnAnyHealthAttacked;
        private void Start()
        {
            OnAnyHealthAttacked.AddListener(UpdateUnitPerformance);
            OnAnyHealthAttacked.AddListener(LogDamageReport);
        }

        private void UpdateUnitPerformance(DamageReport damageReport)
        {
            UnitController unit = damageReport.damageInfo.attacker.GetComponent<UnitController>();
            UnitController victimUnit = damageReport.victim.GetComponent<UnitController>();
            Actor victimActor = damageReport.victim.GetComponent<Actor>();

            if (unit)
            {
                unit.unitPerformanceLastBattle.timesAttacked++;
                unit.unitPerformanceLastBattle.timesCrit += damageReport.damageInfo.isCrit ? 1 : 0;
                unit.unitPerformanceLastBattle.damageDealt += damageReport.damageDealt;
                unit.unitPerformanceLastBattle.unitsKilled += damageReport.wasLethal && victimUnit ? 1 : 0;
                unit.unitPerformanceLastBattle.actorsKilled += damageReport.wasLethal && victimActor ? 1 : 0;
                unit.performanceUpdatedEvent.Invoke(unit.unitPerformanceLastBattle);

            }
            if (victimUnit)
            {
                victimUnit.unitPerformanceLastBattle.damageRecieved += damageReport.damageDealt;
                victimUnit.unitPerformanceLastBattle.damageBlocked += damageReport.wasBlocked ? damageReport.incomingDamage : 0;
                victimUnit.unitPerformanceLastBattle.damagePassedToActor += damageReport.damageRemainder;
                victimUnit.unitPerformanceLastBattle.timesDied += damageReport.wasLethal ? 1 : 0;
                victimUnit.performanceUpdatedEvent.Invoke(victimUnit.unitPerformanceLastBattle);
            }
        }

        private void LogDamageReport(DamageReport damageReport)
        {
            if (!logDamageReports) return;
            string logString = $"[DAMAGE REPORT]: Unit {damageReport.damageInfo.attacker.name} ";
            logString += damageReport.damageInfo.isCrit ? "CRITICALLY " : "";
            logString += (damageReport.wasLethal) ? "killed " : "attacked ";
            logString += $"opposing {damageReport.victim.name} with {damageReport.incomingDamage} damage ";
            logString += damageReport.damageInfo.isOverflow ? "via overflow " : "";
            logString += damageReport.wasBlocked ? "but was BLOCKED " : "";
            logString += $"({damageReport.healthBeforeDamage}) => ({damageReport.healthAfterDamage}) ";
            Debug.Log(logString);
        }
    }


    public struct DamageInfo
    {
        public float damage;
        public bool isCrit;
        public float critMultiplier;
        public bool isOverflow;
        /// <summary>
        /// who is dealing the damage
        /// </summary>
        public GameObject attacker;
        /// <summary>
        /// what specifically did the damage, like an ability 
        /// </summary>
        public GameObject inflictor;
    }

    /// <summary>
    /// Struct containing info of a damage exchange that just occured
    /// </summary>
    public struct  DamageReport
    {
        public float healthBeforeDamage;
        public float healthAfterDamage;
        public float damageRemainder;
        public DamageInfo damageInfo;
        public float damageDealt;
        public float incomingDamage;
        public bool wasBlocked;
        public bool wasLethal;
        public GameObject victim;
    }


    public struct  UnitPerformance
    {
        public float damageDealt;
        public float damageDealtToActors;
        public float damageDealtToUnits;
        public float damageRecieved;
        public float damageBlocked;
        public float damagePassedToActor;
        public int unitsKilled;
        public int actorsKilled;
        public int timesAttacked;
        public int timesCrit;
        public int timesDug;
        public int timesBlocked;
        public int timesDied;

        public int battlesSurvived;
        public int battlesLost;
        public int battlesWon;

        public static UnitPerformance operator+(UnitPerformance a, UnitPerformance b)
        {
           a.damageDealt += b.damageDealt;
           a.damageDealtToActors += b.damageDealtToActors;
           a.damageDealtToUnits += b.damageDealtToUnits;
           a.damageRecieved += b.damageRecieved;
           a.damageBlocked += b.damageBlocked;
           a.damagePassedToActor += b.damagePassedToActor;
           a.unitsKilled += b.unitsKilled;
           a.actorsKilled += b.actorsKilled;
           a.timesAttacked += b.timesAttacked;
           a.timesCrit += b.timesCrit;
           a.timesDug += b.timesDug;
           a.timesBlocked += b.timesBlocked;
           a.timesDied += b.timesDied;
           
           a.battlesSurvived += b.battlesSurvived;
           a.battlesLost += b.battlesLost;
           a.battlesWon += b.battlesWon;
           return b;
        }
    }
}
