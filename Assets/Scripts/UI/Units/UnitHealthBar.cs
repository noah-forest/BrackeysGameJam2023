using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Health))]
public class UnitHealthBar : MonoBehaviour
{
    [SerializeField] private Slider slider;
    private Health health;

    private void Start()
    {
        health = GetComponent<Health>();
        health.healthChanged.AddListener(UpdateHealthBar);
    }

    public void UpdateHealthBar(float oldHealth, float newHealth)
    {
        slider.value = health.health / health.maxHealth;
    }
}
