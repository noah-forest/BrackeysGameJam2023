using System;
using UnityEngine;

public class UnitStats : MonoBehaviour
{
	public float attackPower;
	public float attackInterval;
	[Range(0, 1)] public float blockChance;
	[Range(0, 1)] public float critChance;
	public float critDamage;
	public int digCount;

	public int sellValue;

	[TextArea(5, 30)]
	public string description;

	public void LevelUpUnit()
	{
		attackPower *= 1.25f;
		attackInterval *= 0.95f;
		blockChance *= 1.25f;
		critChance *= 1.25f;
		critDamage *= 1.25f;
		sellValue *= 2;
		digCount++;
	}
}
