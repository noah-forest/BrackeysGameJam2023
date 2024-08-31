using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New StandardEncounter", menuName = "Boat Game/Boss Encounter")]
public class BossEncounterSO : ScriptableObject
{
    [Serializable]
    public struct TeamUnit
    {
        [Range(1,3)]
        public int level;
        public int unitID;
    }

    public TeamUnit[] bossTeam = new TeamUnit[3];
}
