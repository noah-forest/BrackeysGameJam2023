using Assets.Scripts.BoatScripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoatHazard : MonoBehaviour, IBoatInteractable
{
    protected BoatWorldManager boatManager;

    private void Start()
    {
        boatManager = BoatWorldManager.singleton;
    }
    public virtual void InteractWithBoat(BoatMaster boat)
    {
        Debug.Log($"{boatManager} :: {boat}");
        if (!boatManager) boatManager = BoatWorldManager.singleton;
        boatManager.boatSpawn = boat.transform.position;
        Debug.Log($"Boat impacted {gameObject.name}");
    }
}
