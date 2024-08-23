using Assets.Scripts.Units;
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

	public Health target;
	public UnityEvent critEvent;
	UnitStats stats;

	public AudioSource hitAudioPlayer;

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
		DamageInfo damageInfo = new DamageInfo();
		damageInfo.damage = stats.damage;
		damageInfo.isCrit = (Random.value < stats.critChance); // determine whether they crit
        damageInfo.critMultiplier = stats.critDamageMult;
		damageInfo.attacker = gameObject;
		damageInfo.inflictor = gameObject;

        if (damageInfo.isCrit) critEvent.Invoke();
		target.TakeDamage(damageInfo);
		hitAudioPlayer.PlayOneShot(hitAudioPlayer.clip);
	}
	public void Respawn()
	{

	}
}
