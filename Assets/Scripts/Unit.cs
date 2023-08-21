using System;
using System.Net.Sockets;
using UnityEngine;

[RequireComponent(typeof(UnitStats))]
[RequireComponent(typeof(Health))]
public class Unit : MonoBehaviour
{
	public Unit unitToAttack;
	public UnitStats stats;
	public Health health;
	
	//private Actor actor; -- tbd

	private void Start()
	{
		stats = GetComponent<UnitStats>();
		health.died.AddListener(OnDied);
	}

	private void AttackUnit()
	{
		// attack the unit to attack
		health.Damage(stats.attackPower);
	}


	private void OnDied()
	{
		// go to grave
		gameObject.SetActive(false);
	}

	public void Respawn()
	{
		// come back from grave
		gameObject.SetActive(true);
	}
}
