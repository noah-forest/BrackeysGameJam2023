using System;
using System.Collections;
using System.Net.Sockets;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

/// <summary>
/// Handles damaging the oposing unit and enemy actor
/// </summary>
[RequireComponent(typeof(UnitStats))]
[RequireComponent(typeof(Health))]
[RequireComponent(typeof(UnitAnimator))]
public class UnitAttacker : MonoBehaviour
{
	public UnitAttacker unitAttackerToAttack;
	[HideInInspector]
	public UnitStats stats;
	[HideInInspector]
	public Health health;
	public Actor enemyActor;

	public UnityEvent attacked;

	private bool isAttacking;
	private bool crit;
	private bool blocked;

	private void Start()
	{
		stats = GetComponent<UnitStats>();
		health = GetComponent<Health>();
	}

	public void AttackUnit()
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
		unitAttackerToAttack.Damage(stats.attackPower, crit, blocked);
	}

	private void Damage(float dmg, bool crit, bool blocked)
	{
		// if the unit is dead, damage the actor
		if (health.isDead)
		{
			enemyActor.health.Damage(stats.attackPower);
			return;
		}
		
		// damage the unit
		if (!blocked)
		{
			float damageToDeal = stats.attackPower;
			if (crit)
			{
				damageToDeal = stats.attackPower + stats.critDamage;
			}
			health.Damage(damageToDeal);
			attacked.Invoke();
		};
	}
	public void Respawn()
	{
		// come back from grave
		isAttacking = false;
	}
}
