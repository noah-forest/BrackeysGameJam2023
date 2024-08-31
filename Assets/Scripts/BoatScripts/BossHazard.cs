using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewBehaviourScript : BoatHazard
{
    [SerializeField]
    BossEncounterSO bossEncounter;
    public override void InteractWithBoat(BoatMaster boat)
    {
        base.InteractWithBoat(boat);
        boat.gameManager.startShopTransition.Invoke();
        if (bossEncounter)
        {
            boat.gameManager.battleManager.nextBossEncounter = bossEncounter;
        }
        boat.gameManager.battleManager.nextBattleDifficulty = BattleDifficulty.superHard;
        Destroy(boatManager.gameObject);
    }
}
