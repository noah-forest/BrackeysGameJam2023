using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grave : MonoBehaviour
{
	public Unit unit;

	private bool inGrave;
	private int currentDigCount;

	private void Start()
	{
		unit.health.died.AddListener(UnitDied);
	}

	private void UnitDied()
	{
		inGrave = true;
	}

	public void Dig()	//called by button onClick function
	{
		if(!inGrave) return;
		currentDigCount++;
		if (currentDigCount >= unit.stats.digCount)
		{
			inGrave = false;
			unit.Respawn();
		}
	}
}
