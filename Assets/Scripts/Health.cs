using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Health : MonoBehaviour
{
	public int maxHealth;
	public int health;
	public bool isDead;
	public UnityEvent died;

	private void Start()
	{
		health = maxHealth;
	}

	public void Damage(int dmg)
	{
		health -= dmg;
		if (health <= 0)
		{
			Die();
		}
	}

	public void Revive()
	{
		isDead = false;
		health = maxHealth;
	}

	private void Die()
	{ 
		Debug.Log(gameObject.name + " died");
		died.Invoke();
		isDead = true;
	}
}
