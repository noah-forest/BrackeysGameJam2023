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

	public HealthBar healthBar;

	private void Start()
	{
		Revive();
	}

	public void Damage(int dmg)
	{
		health -= dmg;
		healthBar.UpdateHealthBar(health, maxHealth);
		if (health <= 0)
		{
			Die();
		}
	}

	public void Revive()
	{
		isDead = false;
		health = maxHealth;
		healthBar.UpdateHealthBar(health, maxHealth);
	}

	private void Die()
	{
		died.Invoke();
		isDead = true;
	}
}
