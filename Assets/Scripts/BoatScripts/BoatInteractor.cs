using Assets.Scripts.BoatScripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Handles boat interactions
/// </summary>
public class BoatInteractor : MonoBehaviour
{
    public BoatMaster boat;

    private void OnCollisionEnter(Collision collision)
    {
        collision.gameObject.GetComponent<IBoatInteractable>()?.InteractWithBoat(boat, collision);
    }
}
