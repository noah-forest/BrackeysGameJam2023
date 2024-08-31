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

	bool idSet;
	[SerializeField]
	int _unitID;
	/// <summary>
	/// assignment will fail after first time. ID should be assigned by unit manager and nowehere else
	/// </summary>
	public int UnitID
	{
		get { return _unitID; }
		set
		{
			if (!idSet)
			{
				_unitID = value;
				idSet = true;
			}
		}
	}

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
