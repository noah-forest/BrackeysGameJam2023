using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadExcel : MonoBehaviour
{
	public UnitInfo blankItem;
	public UnitManager unitManager;

	public GameManager gameManager;

	private Dictionary<string, UnitRarity> unitRaritys = new Dictionary<string, UnitRarity>()
	{
		{"Common", UnitRarity.Common},
		{"Rare", UnitRarity.Rare},
		{"Epic", UnitRarity.Epic},
		{"Legendary", UnitRarity.Legendary}
	};

	public void LoadUnitData()
	{
		// clear database
		unitManager.unitStatsDatabase.Clear();
		List<Dictionary<string, object>> data;

		//read CSV files
		if (gameManager.oldStatValues)
		{
			data = CSVReader.Read("OldUnitStats");
		} else
		{
			data = CSVReader.Read("UnitStats");
		}

		for(int i = 0; i < data.Count; i++)
		{
			int Health = int.Parse(data[i]["Health"].ToString(), System.Globalization.NumberStyles.Integer);
			int Damage = int.Parse(data[i]["Damage"].ToString(), System.Globalization.NumberStyles.Integer);
			float AttackSpeed = float.Parse(data[i]["Attack Speed"].ToString(), System.Globalization.NumberStyles.Float);
			float BlockChance = float.Parse(data[i]["Block Chance"].ToString(), System.Globalization.NumberStyles.Float);
			float CritChance = float.Parse(data[i]["Crit Chance"].ToString(), System.Globalization.NumberStyles.Float);
			int DigCount = int.Parse(data[i]["Dig Count"].ToString(), System.Globalization.NumberStyles.Integer);
			string Name = data[i]["Name"].ToString();
			UnitRarity Rarity = unitRaritys[data[i]["Rarity"].ToString()];
			string Description = data[i]["Description"].ToString();
			Description = Description.Replace("[", "\n");

			AddItem(Health, Damage, AttackSpeed, BlockChance, CritChance, DigCount, Name, Rarity, Description);
		}

		unitManager.unitStatsDatabase.Sort(delegate (UnitInfo a, UnitInfo b)
		{
			return a.rarity > b.rarity ? 1 : -1;
		});
	}

	private void AddItem(int health, int damage, float attackSpeed, float blockChance, float critChance, int digCount, string name, UnitRarity rarity, string description)
	{
		UnitInfo tempItem = new UnitInfo(blankItem);

		tempItem.health = health;
		tempItem.damage = damage;
		tempItem.attackSpeed = attackSpeed * 0.1f;
		tempItem.blockChance = blockChance * .01f;
		tempItem.critChance = critChance * .01f;
		tempItem.digCount = digCount;
		tempItem.name = name;
		tempItem.rarity = rarity;
		tempItem.description = description;
		tempItem.unitSprite = Resources.Load<Sprite>($"UnitSprites/{name}");

		unitManager.unitStatsDatabase.Add(tempItem);
	}
}
