using System;
using UnityEngine;
using glumpis.CharacterStats;

public enum UnitRarity
{
	None = 1,
	Common = 2,
	Rare,
	Epic,
	Legendary
}

public class UnitStats : MonoBehaviour
{
	public UnitRarity Rarity;

	public CharacterStat damage;
	public CharacterStat attackSpeed;
	public CharacterStat blockChance;
	public CharacterStat critChance;
	public CharacterStat critDamageMult;
	public CharacterStat digCount;

	public CharacterStat sellValue;

	[TextArea(5, 30)]
	public string description;

	public Health health;

	public void InitUnit(UnitInfo unitInfo)
	{
		health.InitHealth(unitInfo.health);
		damage = new CharacterStat(unitInfo.damage, 0);
		attackSpeed = new CharacterStat(unitInfo.attackSpeed, 0);
		critDamageMult = new CharacterStat(2, 1);
		critChance = new CharacterStat(unitInfo.critChance, 0, 1);
		blockChance = new CharacterStat(unitInfo.blockChance, 0, 1);
		digCount = new CharacterStat(unitInfo.digCount, 1);
		sellValue = new CharacterStat(1, 1);
	}

	public void LevelUpUnit(object source)
	{
		StatModifier levelUpMod = new(0.25f, StatModType.PercentMult, (int)StatModType.PercentMult - 1, source);

		health.maxHealth += levelUpMod;

		damage += levelUpMod;
		attackSpeed += new StatModifier(0.05f, StatModType.PercentMultReduction, (int)StatModType.PercentMultReduction - 1, source);
		
		critChance += levelUpMod;
		blockChance += levelUpMod;
		
		digCount += new StatModifier(1, StatModType.Flat, (int)StatModType.Flat - 1, source);
		sellValue += new StatModifier(2, StatModType.PercentMult, (int)StatModType.PercentMult - 1, source);
	}
}	
