using UnityEngine;

[System.Serializable]
public class UnitInfo
{
	public string name;
	public int health;
	public int damage;
	public float attackSpeed;
	public float blockChance;
	public float critChance;
	public int digCount;
	public UnitRarity rarity;
	public string description;
	public Sprite unitSprite;

	public UnitInfo(UnitInfo d)
	{
		unitSprite = d.unitSprite;
		name = d.name;
		health = d.health;
		damage = d.damage;
		attackSpeed = d.attackSpeed;
		blockChance = d.blockChance;
		critChance = d.critChance;
		digCount = d.digCount;
		rarity = d.rarity;
		description = d.description;
	}
}
