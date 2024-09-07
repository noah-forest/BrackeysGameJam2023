using Assets.Scripts.BoatScripts;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BoatHazard : MonoBehaviour, IBoatInteractable
{
    protected BoatWorldManager boatManager;
    [SerializeField] protected GameObject impactEffect;
    /// <summary>
    /// The higher this value, the more 'difficult' it will be for the path to go through this hazard, however 0 will make the tile impassible
    /// </summary>
    public float pathFindingInfluence = 1;

    private void Start()
    {
        boatManager = BoatWorldManager.singleton;
    }
    public virtual void InteractWithBoat(BoatMaster boat, ControllerColliderHit hit)
    {
        //Debug.Log($"{boatManager} :: {boat}");
        if (!boatManager) boatManager = BoatWorldManager.singleton;
        boatManager.boatSpawn = boat.transform.position;
        boatManager.boatLaneSpawn = boat.controller.CurrentLane;
        //Debug.Log($"Boat impacted {gameObject.name}");
        if(impactEffect) Instantiate(impactEffect, transform.position, transform.rotation);

    }
}
