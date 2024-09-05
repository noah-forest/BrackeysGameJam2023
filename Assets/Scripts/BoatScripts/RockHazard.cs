using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RockHazard : BoatHazard
{
    [SerializeField] bool destroyOnImpact;
    public override void InteractWithBoat(BoatMaster boat, ControllerColliderHit hit)
    {
        base.InteractWithBoat(boat, hit);
        boat.controller.Bounce();
        if(destroyOnImpact) Destroy(gameObject);
    }
}
