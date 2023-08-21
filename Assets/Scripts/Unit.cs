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
		Debug.Log(gameObject.name + " is not dead");
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
			Debug.Log(this.name + "'s Health is: " + health.health);
			yield return new WaitForSeconds(stats.attackInterval);
		}

		isAttacking = false; // unit is dead
		yield return null;
	}

	private void AttackUnit()
	{
		Debug.Log("Attacking: " + unitToAttack.name);
		unitToAttack.Damage(stats.attackPower);
	}

	private void Damage(int dmg)
	{
		//if (health.isDead) actor.health.Damage(stats.attackPower); // if the unit is dead, attack the actor
		health.Damage(stats.attackPower);
		Debug.Log(gameObject.name + " dealt " + stats.attackPower + " damage.");
	}


	private void OnDied()
	{
		// go to grave
		Debug.Log(gameObject.name + " died.");
		Debug.Log(unitToAttack.gameObject.name + " has " + unitToAttack.health.health + " health left.");
		gameObject.SetActive(false);
	}

	public void Respawn()
	{
		// come back from grave
		gameObject.SetActive(true);
		health.Revive();
	}
}
