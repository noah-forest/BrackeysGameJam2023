using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActorHealth : MonoBehaviour
{
	public HealthBar healthBar;
	private Health health;

	private void Start()
	{
		health = GetComponent<Health>();
		health.healthChanged.AddListener(UpdateHealthBar);
	}

	public void UpdateHealthBar(float oldHealth, float newHealth)
	{
		healthBar.gameObject.GetComponent<Renderer>().material.SetFloat("_healthNormalized", health.health / health.maxHealth);
		healthBar.gameObject.GetComponent<Renderer>().material.SetColor("_fillColor", healthBar.lowToHighHealthTransition.Evaluate(health.health / health.maxHealth));
	}
}
