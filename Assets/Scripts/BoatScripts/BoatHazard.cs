using Assets.Scripts.BoatScripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoatHazard : MonoBehaviour, IBoatInteractable
{
    public void InteractWithBoat(BoatMaster boat)
    {
        Debug.Log($"Boat impacted {gameObject.name}");
    }
}
