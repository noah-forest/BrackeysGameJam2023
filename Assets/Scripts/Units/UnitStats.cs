using System;
using UnityEngine;

public enum UnitRarity
{
	Common = 2,
	Rare,
	Epic,
	Legendary
}

public class UnitStats : MonoBehaviour
{
	public UnitRarity Rarity;

	public float attackPower;
	public float attackInterval;
	[Range(0, 1)] public float blockChance;
	[Range(0, 1)] public float critChance;
	public float critDamage;
	public int digCount;

	public int sellValue;

	[TextArea(5, 30)]
	public string description;

	private Health health;

	public void LevelUpUnit()
	{
		health = GetComponent<Health>();

		health.maxHealth *= 1.25f;

		attackPower *= 1.25f;
		attackInterval *= 0.95f;
		blockChance *= 1.25f;
		critChance *= 1.25f;
		critDamage *= 1.25f;
		sellValue *= 2;
		digCount++;
	}
}
