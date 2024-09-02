using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleLane : MonoBehaviour
{
	public GameObject enemyUnit;
	/// <summary>
	/// left side of battlefield
	/// </summary>
	public UnitController enemyUnitController;
    /// <summary>
    /// left side of battlefield
    /// </summary>
    public Transform enemyUnitPosition;
    /// <summary>
    /// left side of battlefield
    /// </summary>
    public Grave enemyGrave;

    /// <summary>
    /// right side of battlefield
    /// </summary>
    public UnitController playerUnit;
    /// <summary>
    /// right side of battlefield
    /// </summary>
    public Transform playerUnitPosition;
    /// <summary>
    /// right side of battlefield
    /// </summary>
    public Grave playerGrave;

    public int LaneID;

}
