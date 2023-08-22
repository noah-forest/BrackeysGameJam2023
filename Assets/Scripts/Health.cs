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
			_health = value;
			healthChanged.Invoke();
		}
	}

	public bool isDead;
	public UnityEvent died;
	public UnityEvent healthChanged;

	private void Start()
	{
		Revive();
	}

	public void Damage(float dmg)
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
