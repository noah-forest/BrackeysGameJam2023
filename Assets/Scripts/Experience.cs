using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Experience : MonoBehaviour
{
	public static int MaxLevel = 3;
	public static int ExpToLevel2 = 2;
	public static int ExpToLevel3 = 3;
	public int curLevel = 1; //default lvl is 1
	public int _exp;
	[HideInInspector] public UnityEvent<int> expGained = new();
	[HideInInspector] public UnityEvent<int> unitLevelUp = new();

	private static Dictionary<int, int> levelToExpNeeded = new()
	{
		{1, ExpToLevel2},
		{2, ExpToLevel3}
	};

	public int Exp
	{
		get => _exp;
		set
		{
			int tempExp = _exp;
			_exp = value;
			if (_exp == 0) return;
			expGained.Invoke(_exp - tempExp);
			TryToLevel();
		}
	}

	public void AddExp(int xp)
	{
		if (curLevel == MaxLevel) return;
		Exp += xp;
		Debug.Log($"current xp is: {Exp}");
	}

	private void LevelUp()
	{
		++curLevel;

		Debug.Log($"is now level {curLevel}");

		UnitStats unitStats = GetComponent<UnitStats>();
		unitStats.LevelUpUnit();
	}

	private void TryToLevel()
	{
		// level up conditions
		if (curLevel == 1 && Exp >= ExpToLevel2)
		{
			Exp -= ExpToLevel2;
			LevelUp();
			unitLevelUp.Invoke(curLevel);
		}

		if (curLevel == 2 && Exp >= ExpToLevel3)
		{
			Exp = 0;
			LevelUp();
			unitLevelUp.Invoke(curLevel);
		}
	}

	public int GetExpNeeded()
	{
		if (levelToExpNeeded.ContainsKey(curLevel))
		{
			return levelToExpNeeded[curLevel];
		}
        else
        {
             return levelToExpNeeded[MaxLevel-1];
        }
    }
}
