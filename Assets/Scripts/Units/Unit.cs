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
	[HideInInspector]
	public UnitStats stats;
	[HideInInspector]
	public Health health;
	public Actor actor;

	private GameManager gameManager;
	
	private bool isAttacking;
	private bool crit;
	private bool blocked;

	private void Start()
	{
		gameManager = GameManager.singleton;
		stats = GetComponent<UnitStats>();
		health = GetComponent<Health>();
		health.died.AddListener(OnDied);
	}

	private void Update()
	{
		if (health.isDead) return;	
		if (isAttacking) return;
		
		StartCoroutine(AttackLoop());
	}

	private IEnumerator AttackLoop()
	{
		isAttacking = true;
		while (!health.isDead && !gameManager.resolved)
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
			// crit hit
			blocked = false;
			crit = true;
		} else if (randValue < stats.blockChance)
		{
			// blocked hit
			crit = false;
			blocked = true;
		}
		else
		{
			// normal hit
			crit = false;
			blocked = false;
		}
		
		unitToAttack.Damage(stats.attackPower, crit, blocked);
	}

	private void Damage(float dmg, bool crit, bool blocked)
	{
		// if the unit is dead, damage the actor
		if (health.isDead)
		{
			actor.health.Damage(stats.attackPower);
			return;
		}
		
		// damage the unit
		if (blocked) return;
		if(crit) health.Damage(stats.attackPower + stats.critDamage);
		else health.Damage(stats.attackPower);
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
