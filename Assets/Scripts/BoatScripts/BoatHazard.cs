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
        boatManager.boatSpawn = boat.transform.position;
        Debug.Log($"Boat impacted {gameObject.name}");
    }
}
