using System;
using System.Collections;
using System.Net.Sockets;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(UnitStats))]
[RequireComponent(typeof(Health))]
public class Unit : MonoBehaviour
{
	public Unit unitToAttack;
	public UnitStats stats;
	public Health health;

	public Actor actor;
	
	private bool isAttacking;

	private void Start()
	{
		stats = GetComponent<UnitStats>();
		health.died.AddListener(OnDied);
	}

	private void Update()
	{
		if (health.isDead) return;
		if (!isAttacking)
		{
			StartCoroutine(AttackLoop()); 
		}
	}

	private IEnumerator AttackLoop()
	{
		isAttacking = true;
		while (!health.isDead)
		{
			AttackUnit();
			yield return new WaitForSeconds(2);
		}

		isAttacking = false; // unit is dead
		yield return null;
	}

	private void AttackUnit()
	{
		unitToAttack.Damage(stats.attackPower);
	}

	private void Damage(int dmg)
	{
		if (health.isDead)
		{
			actor.health.Damage(stats.attackPower); // if the unit is dead, attack the actor
			return;
		}
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
		health.Revive();
		isAttacking = false;
	}
}
