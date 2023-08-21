using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Health : MonoBehaviour
{
	public int health;
	public UnityEvent died;

	public void Damage(int dmg)
	{
		health -= dmg;
		if (health <= 0)
		{
			Die();
		}
	}

	private void Die()
	{
		died.Invoke();
	}
}
