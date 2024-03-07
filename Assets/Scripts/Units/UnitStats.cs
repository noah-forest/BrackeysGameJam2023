using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;

public class UnitStats : MonoBehaviour
{
	public float attackPower;
	public float attackInterval;
	[Range(0, 1)] public float blockChance;
	[Range(0, 1)] public float critChance;
	public float critDamage;
	public int digCount;

	[TextArea(5, 30)]
	public string description;

	[HideInInspector] public List<float> stats = new();

	private void Awake()
	{
		stats.Add(attackPower);
		stats.Add(attackInterval);
		stats.Add(blockChance);
		stats.Add(critChance);
		stats.Add(critDamage);
		stats.Add(digCount);
	}
}
