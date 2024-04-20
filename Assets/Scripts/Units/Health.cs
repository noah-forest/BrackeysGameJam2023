using UnityEngine;
using UnityEngine.Events;
using glumpis.CharacterStats;

public class Health : MonoBehaviour
{
	public CharacterStat maxHealth;
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
		if (health <= 0) return;

		health -= dmg;
		if (health <= 0.1)
		{
			health = 0;
			isDead = true;
			Die();
		}
	}

	public void InitHealth(int health)
	{
		maxHealth = new CharacterStat(health, 1);
	}

	public void Revive()
	{
		isDead = false;
		health = maxHealth;
	}

	private void Die()
	{
		died.Invoke();
	}
}
