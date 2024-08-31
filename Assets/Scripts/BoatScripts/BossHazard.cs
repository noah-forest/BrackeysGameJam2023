using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewBehaviourScript : BoatHazard
{
    public override void InteractWithBoat(BoatMaster boat)
    {
        base.InteractWithBoat(boat);
        boat.gameManager.startShopTransition.Invoke();
        boat.gameManager.battleManager.nextBattleDifficulty = BattleDifficulty.superHard;
        Destroy(boatManager.gameObject);
    }
}
