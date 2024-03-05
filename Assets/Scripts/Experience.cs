using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine;
using Unity.VisualScripting;

public class Experience : MonoBehaviour
{
	public int curLevel = 1; //default lvl is 1
	public int _exp;
	[HideInInspector] public UnityEvent expChanged;

	public int Exp
	{
		get => _exp; 
		set
		{
			_exp = value;
			expChanged.Invoke();
		}
	}

	private void Awake()
	{
		expChanged.AddListener(ExpChanged);
	}

	public void AddExp()
	{
		if (curLevel == 3) return;
		++Exp;
		Debug.Log($"current xp is: {Exp}");
	}

	private void LevelUp()
	{
		++curLevel;
		Debug.Log($"is now level {curLevel}");

		UnitStats unitStats = GetComponent<UnitStats>();
		unitStats.attackPower++;
	}

	private void ExpChanged()
	{
		//ui stuff


		// level up conditions
		if(Exp == 2)
		{
			LevelUp();
		}

		if(curLevel == 2 && Exp == 5)
		{
			LevelUp();
		}
	}
}
