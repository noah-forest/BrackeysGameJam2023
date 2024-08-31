using System.Collections;
using System.Collections.Generic;
using UnityEditor.ShaderGraph.Serialization;
using UnityEngine;

/// <summary>
/// connects all boat scripts
/// </summary>
public class BoatMaster : MonoBehaviour
{
    public BoatController controller;
    public BoatInteractor impactLogic;
    public GameManager gameManager;
    public BoatWorldManager boatManager;

    private void Start()
    {
        gameManager = GameManager.singleton;
        boatManager = BoatWorldManager.singleton;

        boatManager.boat = this;
        
        controller.orb.isKinematic = true;
        transform.position = boatManager.boatSpawn + boatManager.boatOffset;
        controller.orb.isKinematic = false;

    }
}
