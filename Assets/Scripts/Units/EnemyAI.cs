using System;
using System.Collections;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
	public Unit enemyUnit;

	private float timer;
	private int seconds;
	private bool dead;
	
	private void Start()
	{
		enemyUnit.health.died.AddListener(OnDeath);
	}

	private void OnDeath()
	{
		Debug.Log("Enemy unit died.");
		dead = true;
		timer = 0.0f;
	}

	private void Update()
	{
		if (!dead) return;

		timer += Time.deltaTime;
		seconds = (int)(timer % 60);

		if (seconds < enemyUnit.stats.digCount) return;
		enemyUnit.Respawn();
		dead = false;
	}
}
