using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum BattleDifficulty
{
    easy,
    medium,
    hard,
    superHard
}

public class BattleHazard : BoatHazard
{
    public BattleDifficulty battleDifficulty;
    [SerializeField]
    GameObject[] difficultyVisuals;
    public void Start()
    {
        difficultyVisuals[(int)battleDifficulty].SetActive(true);
    }

    public override void InteractWithBoat(BoatMaster boat, ControllerColliderHit hit)
    {
        base.InteractWithBoat(boat, hit);
        if (boat.gameManager)
        {
            boat.controller.StopMovement();
            boat.gameManager.battleManager.nextBattleDifficulty = battleDifficulty;
            boat.gameManager.startShopTransition.Invoke();
        }
        Destroy(gameObject);
    }
}
