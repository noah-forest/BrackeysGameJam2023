using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// connects all boat scripts
/// </summary>
public class BoatMaster : MonoBehaviour
{
    public BoatController controller;
    public BoatInteractor impactLogic;
    public GameManager gameManager;

    private void Start()
    {
        gameManager = GameManager.singleton;
    }
}
