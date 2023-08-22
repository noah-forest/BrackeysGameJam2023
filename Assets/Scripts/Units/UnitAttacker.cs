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

	[HideInInspector]
	public UnitStats stats;
	[HideInInspector]
	public Health health;
	public Actor parentActor;
	public UnitAttacker targetUnit;

	public UnityEvent attacked;

	private bool isAttacking;
	private bool crit;
	private bool blocked;

	private void Start()
	{
		stats = GetComponent<UnitStats>();
		health = GetComponent<Health>();
	}

	/// <summary>
	/// attack this units targetUnit using targetUnit.takeDamage
	/// </summary>
	public void AttackTarget()
	{

		// Damage instance feilds.

		float damage = stats.attackPower;

		crit = false;       // currently cirt bool does nothing ,but may be used later.
		float critRoll = Random.value;
		if (critRoll < stats.critChance) // determine whether they crit
		{
			// crit hit
			//crit = true;
			damage += stats.attackPower;

		}

		targetUnit.TakeDamage(damage);
	}

	/// <summary>
	/// called by other units in order to deal damage to THIS unit
	/// </summary>
	/// <param name="dmg"></param>
	/// <param name="crit"></param>
	/// <param name="blocked"></param>
	private void TakeDamage(float dmg)
	{
		// if the unit is dead when it would normally take damage, damage this units actor
		if (health.isDead)
		{
			parentActor.health.TakeDamage(dmg);
			return;
		}

		//check if the unit blocks the incoming damage
		blocked = false;
		float blockRoll = Random.value;
		if (blockRoll < stats.blockChance)
		{
			// blocked hit
			blocked = true;
		} // this is seperated out from the follwing 'damage the unit if' as other behavior may rely on block status.

		// damage the unit
		if (!blocked)
		{
			health.TakeDamage(dmg);
			attacked.Invoke();
		};
	}
	public void Respawn()
	{
		// come back from grave
		isAttacking = false;
	}
}
