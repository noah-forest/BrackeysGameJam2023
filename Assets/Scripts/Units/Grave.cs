using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[RequireComponent(typeof(Collider2D))]
public class Grave : MonoBehaviour
{
	[FormerlySerializedAs("unit")] public UnitAttacker unitAttacker;

	private bool inGrave;
	private int currentDigCount;

	private void Start()
	{
		unitAttacker.gameObject.GetComponent<Health>().died.AddListener(UnitDied);
	}

	private void UnitDied()
	{
		inGrave = true;
		currentDigCount = 0;
		Debug.Log(unitAttacker.name + " is in grave");
	}

	public void Dig()	//called by button onClick function
	{
		if(!inGrave) return;
		currentDigCount++;
        
		if (currentDigCount >= unitAttacker.stats.digCount)
		{
			inGrave = false;
			unitAttacker.Respawn();
			Debug.Log("Respawned");
		}
	}
	
	void OnMouseDown() // when collider is clicked
	{
		Dig();
	}
}
