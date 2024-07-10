using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class UnitManager : MonoBehaviour
{
	public List<UnitInfo> unitStatsDatabase;

	public Dictionary<UnitRarity, int> unitRarityCount = new()
	{
		{UnitRarity.None, 0},
		{UnitRarity.Common, 0},
		{UnitRarity.Rare, 0},
		{UnitRarity.Epic, 0},
		{UnitRarity.Legendary, 0}
	};

	public Dictionary<UnitRarity, int> rarityOffsets = new()
	{
		{UnitRarity.None, 0},
		{UnitRarity.Common, 0},
		{UnitRarity.Rare, 0},
		{UnitRarity.Epic, 0},
		{UnitRarity.Legendary, 0}
	};

	private void Start()
	{
		UnitRarity curRarity = UnitRarity.None;

		for (int i = 0; i < unitStatsDatabase.Count; i++)
		{
			UnitInfo unit = unitStatsDatabase[i];

			++unitRarityCount[unit.rarity];
			
			if(curRarity != unit.rarity)
			{
				curRarity = unit.rarity;
				rarityOffsets[curRarity] = i;
			}
		}
	}
}
