using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitManager : MonoBehaviour
{
	public List<UnitInfo> unitStatsDatabase;

	public List<Dictionary<UnitRarity, int>> unitRarityCount = new();
}
