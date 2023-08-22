using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class Grave : MonoBehaviour
{
	public Unit unit;

	private bool inGrave;
	private int currentDigCount;

	private void Start()
	{
		unit.gameObject.GetComponent<Health>().died.AddListener(UnitDied);
	}

	private void UnitDied()
	{
		inGrave = true;
		currentDigCount = 0;
		Debug.Log(unit.name + " is in grave");
	}

	public void Dig()	//called by button onClick function
	{
		if(!inGrave) return;
		currentDigCount++;
        
		if (currentDigCount >= unit.stats.digCount)
		{
			inGrave = false;
			unit.Respawn();
			Debug.Log("Respawned");
		}
	}
	
	void OnMouseDown() // when collider is clicked
	{
		Dig();
	}
}
