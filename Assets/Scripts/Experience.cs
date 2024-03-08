using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine;
using Unity.VisualScripting;

public class Experience : MonoBehaviour
{
	public static int MaxLevel = 3;
	public static int ExpToLevel2 = 2;
	public static int ExpToLevel3 = 5;
	public int curLevel = 1; //default lvl is 1
	public int _exp;
	[HideInInspector] public UnityEvent<int> expGained = new();
	[HideInInspector] public UnityEvent<int> unitLevelUp = new();

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
		Exp = 0;
		Debug.Log($"is now level {curLevel}");

		UnitStats unitStats = GetComponent<UnitStats>();
		unitStats.attackPower++;
	}

	private void TryToLevel()
	{
		// level up conditions
		if(curLevel == 1 && Exp == ExpToLevel2)
		{
			LevelUp();
			unitLevelUp.Invoke(curLevel);
		}

		if(curLevel == 2 && Exp == ExpToLevel3)
		{
			LevelUp();
			unitLevelUp.Invoke(curLevel);
		}
	}
}
