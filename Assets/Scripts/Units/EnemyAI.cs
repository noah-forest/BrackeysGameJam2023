using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;

public class EnemyAI : MonoBehaviour
{
	[FormerlySerializedAs("enemyUnit")] public UnitAttacker enemyUnitAttacker;

	private float timer;
	private int seconds;
	private bool dead;
	
	private void Start()
	{
		enemyUnitAttacker.health.died.AddListener(OnDeath);
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

		if (seconds < enemyUnitAttacker.stats.digCount) return;
		enemyUnitAttacker.Respawn();
		dead = false;
	}
}
