using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Health))]
public class UnitHealthBar : MonoBehaviour
{
	public GameObject shadowDefault;
	public GameObject shadowFlipped;

	[SerializeField] private Image bar;
	[SerializeField] private float timeToDrain = 0.25f;
	[SerializeField] private Gradient healthBarColor;
    private Health health;

	private float _target = 1;

	private Color newBarColor;

	private void OnEnable()
	{
		health = GetComponent<Health>();

		bar.color = healthBarColor.Evaluate(_target);
		CheckGradientAmount();

		health.healthChanged.AddListener(UpdateHealthBar);
	}

	public void UpdateHealthBar(float oldHealth, float newHealth)
    {
		_target = health.health / health.maxHealth;

		bar.fillAmount = _target;

		if (gameObject.activeSelf)
		{
			StartCoroutine(DrainHealth());
		}

		CheckGradientAmount();

	}

	private IEnumerator DrainHealth()
	{
		float fillAmount = bar.fillAmount;
		Color currentColor = bar.color;

		float elaspedTime = 0f;
		while(elaspedTime < timeToDrain)
		{
			elaspedTime += Time.deltaTime;

			bar.fillAmount = Mathf.Lerp(fillAmount, _target, elaspedTime / timeToDrain);

			bar.color = Color.Lerp(currentColor, newBarColor, elaspedTime / timeToDrain);

			yield return null;
		}
	}

	private void CheckGradientAmount()
	{
		newBarColor = healthBarColor.Evaluate(_target);
	}
}
