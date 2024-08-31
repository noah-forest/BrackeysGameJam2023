using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleHazard : BoatHazard
{
    public override void InteractWithBoat(BoatMaster boat)
    {
        base.InteractWithBoat(boat);
        boat.gameManager.startShopTransition.Invoke();
        Destroy(gameObject);
    }
}
