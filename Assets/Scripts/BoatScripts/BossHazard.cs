using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NewBehaviourScript : BoatHazard
{
    BossEncounterSO bossEncounter;
    [SerializeField]
    List<BossEncounterSO> possibleEncounters = new List<BossEncounterSO>();
    [SerializeField] SpriteRenderer apearance;

    private void Start()
    {
        if(possibleEncounters.Count > 0)
        {
            bossEncounter = possibleEncounters[Random.Range(0, possibleEncounters.Count)];
            apearance.sprite = bossEncounter.bossPortrait;
        }
    }
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
