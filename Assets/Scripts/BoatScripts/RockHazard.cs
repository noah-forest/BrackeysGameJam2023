using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RockHazard : BoatHazard
{
    [SerializeField] bool destroyOnImpact;
    public override void InteractWithBoat(BoatMaster boat, Collision collision)
    {
        base.InteractWithBoat(boat, collision);
        boat.controller.Bounce(collision.GetContact(0).normal);
        if(destroyOnImpact) Destroy(gameObject);
    }
}
