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

	public UnitController targetUnit;
	public UnityEvent critEvent;
	UnitStats stats;


	private bool isAttacking;
	private bool crit;


	private void Start()
	{
		stats = GetComponent<UnitStats>();
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
			critEvent.Invoke();
			damage += stats.attackPower;

		}

		targetUnit.TakeDamage(damage);
	}
	public void Respawn()
	{
		// come back from grave
		isAttacking = false;
	}
}
