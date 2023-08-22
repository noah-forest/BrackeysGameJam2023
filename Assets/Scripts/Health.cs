using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Health : MonoBehaviour
{
	public int maxHealth;
	public float _health;

	public float health
	{
		get => _health;
		set
		{
			float previousHealth = _health;
			_health = value;
			healthChanged.Invoke(previousHealth, value);
		}
	}

	public bool isDead;
	public UnityEvent died;
	public UnityEvent<float, float> healthChanged;

	private void Start()
	{
		Revive();
	}

	public void TakeDamage(float dmg)
	{
		health -= dmg;
		if (health <= 0.1)
		{
			health = 0;
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
		died.Invoke();
		isDead = true;
	}
}
