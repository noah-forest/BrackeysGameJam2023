using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Health))]
public class Actor : MonoBehaviour
{
	// this is the player / enemy
	public Health health;

	private void Update()
	{
		if(!health.isDead) return;
		
	}
}
