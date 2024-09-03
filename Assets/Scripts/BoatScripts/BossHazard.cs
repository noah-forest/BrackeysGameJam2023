using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NewBehaviourScript : BoatHazard
{
    [SerializeField]
    BossEncounterSO bossEncounter;
    public override void InteractWithBoat(BoatMaster boat, Collision collision)
    {
        base.InteractWithBoat(boat, collision);
        if (boat.gameManager)
        {

            boat.gameManager.startShopTransition.Invoke();
            if (bossEncounter)
            {
                boat.gameManager.battleManager.nextBossEncounter = bossEncounter;
            }
            boat.gameManager.battleManager.nextBattleDifficulty = BattleDifficulty.superHard;

        }
        else
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
        Destroy(boatManager.gameObject);
    }
}
