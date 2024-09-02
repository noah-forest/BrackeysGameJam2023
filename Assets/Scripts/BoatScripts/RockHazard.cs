using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RockHazard : BoatHazard
{
    [SerializeField] bool destroyOnImpact;
    public override void InteractWithBoat(BoatMaster boat)
    {
        base.InteractWithBoat(boat);
        boat.controller.Bounce();
        if(destroyOnImpact) Destroy(gameObject);
    }
}
