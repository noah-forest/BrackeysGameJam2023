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
        controller.characterController.enabled = false;
        transform.position = boatManager.boatSpawn;
        controller.CurrentLane = boatManager.grid? boatManager.boatLaneSpawn : 0;
        controller.characterController.enabled = true;
        

    }
}
