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
			Debug.Log(this.name + "'s Health is: " + health.health);
			AttackUnit();
			yield return new WaitForSeconds(10);
		}

		isAttacking = false; // unit is dead
		yield return null;
	}

	private void AttackUnit()
	{
		Debug.Log(gameObject.name + " is attacking " + unitToAttack.name);
		unitToAttack.Damage(stats.attackPower);
	}

	private void Damage(int dmg)
	{
		if (health.isDead)
		{
			actor.health.Damage(stats.attackPower); // if the unit is dead, attack the actor
			Debug.Log(actor.gameObject.name + " has been hit for " + stats.attackPower + " damage.");
			return;
		}
		health.Damage(stats.attackPower);
		Debug.Log(gameObject.name + " has been hit for " + stats.attackPower + " damage.");
	}


	private void OnDied()
	{
		// go to grave
		Debug.Log(unitToAttack.gameObject.name + " has " + unitToAttack.health.health + " health left.");
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
