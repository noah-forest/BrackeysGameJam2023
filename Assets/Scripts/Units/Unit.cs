using System;
using System.Collections;
using System.Net.Sockets;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

[RequireComponent(typeof(UnitStats))]
[RequireComponent(typeof(Health))]
public class Unit : MonoBehaviour
{
	public Unit unitToAttack;
	public UnitStats stats;
	public Health health;

	public Actor actor;
	
	private bool isAttacking;
	private bool crit;
	private bool blocked;

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
			yield return new WaitForSeconds(2);
			AttackUnit();
		}

		isAttacking = false; // unit is dead
		yield return null;
	}

	private void AttackUnit()
	{
		// determine whether they attack, block, or crit
		float randValue = Random.value;
		if (randValue < stats.critChance)
		{
			Debug.Log("Crit!");
			blocked = false;
			crit = true;
		} else if (randValue < stats.blockChance)
		{
			Debug.Log("Blocked!");
			crit = false;
			blocked = true;
		}
		else
		{
			crit = false;
			blocked = false;
		}
		
		unitToAttack.Damage(stats.attackPower, crit, blocked);
	}

	private void Damage(float dmg, bool crit, bool blocked)
	{
		if (blocked) return;
		if (health.isDead)
		{
			if(crit) actor.health.Damage(stats.attackPower + stats.critDamage);
			actor.health.Damage(stats.attackPower); // if the unit is dead, attack the actor
			return;
		}
		if(crit) health.Damage(stats.attackPower + stats.critDamage);
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
