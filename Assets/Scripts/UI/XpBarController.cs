using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class XpBarController : MonoBehaviour
{
	public List<Color> pipColors = new(); //store both colors for pips
	public List<GameObject> pips = new(); //store a list of pips

	private Queue<GameObject> emptyPipQueue = new();

	public GameObject cont; //pip parent

	public GameObject battleSlot;
	private Slot unitSlot;
	private Experience unitExp;

	private void Awake()
	{
		unitSlot = battleSlot.GetComponent<Slot>();
	}

	public void ClearXpBar()
	{

		unitSlot = battleSlot.GetComponent<Slot>();

		if (unitSlot.payload == null)
		{
			Debug.Log("there be no unit");
		} else
		{
			Debug.Log($"Clearing xpBar for: {unitSlot.payload.name}");
		}

		if (pips.Count > 2)
		{
			Debug.Log("unit over level 1 | removing extra pips");
			Destroy(pips[0]);
		}

		Debug.Log("clearing lists");
		emptyPipQueue.Clear();
		pips.Clear();
		if (unitExp)
		{
			Debug.Log("removing event");
			unitExp.expGained.RemoveListener(FillXpBar);
			unitExp = null;
		}
		
	}

	public void InitXpBar()
	{
		unitSlot = battleSlot.GetComponent<Slot>();

		if (unitExp == unitSlot.payload.GetComponent<Experience>())
		{
			Debug.Log("same unit idk");
			return;
		}
		unitExp = unitSlot.payload.GetComponent<Experience>();

		Debug.Log($"Initialzing xpBar for: {unitSlot.payload.name}");
		foreach (Transform child in cont.transform.GetComponentsInChildren<Transform>())
		{
			if (child.name != "cont")
			{
				pips.Add(child.gameObject);
				emptyPipQueue.Enqueue(child.gameObject);
			}
		}

		if (unitExp.curLevel > 1)
		{
			Debug.Log("unit is greater than lvl 1");
			CreateAndExpandPips();
		}
		FillXpBar(unitExp.Exp);

		Debug.Log("i be adding the event now");
		unitExp.expGained.AddListener(FillXpBar);
	}

	private void FillXpBar(int xp)
	{
		Debug.Log($"adding {xp} to addExpPip -----------------------------");
		if (unitExp.curLevel == Experience.MaxLevel) return; //if they're max level, return

		for (int i = 0; i < xp; i++) // foreach experience point gained
		{
			Debug.Log($"expIndex: {i} | totalPips: {pips.Count} | emptyPipsQ: {emptyPipQueue.Count}");
			GameObject pipToFill = emptyPipQueue.Dequeue();

			if (unitExp.curLevel == 1)
			{
				if (unitExp.Exp + i > Experience.ExpToLevel2) //is this greater than the maximum xp for this level
				{
					Debug.Log($"{unitExp.Exp + i} is greater than xp required to lvl | doing rollover ");
					CreateAndExpandPips();
					pipToFill = emptyPipQueue.Dequeue();
				}
				else if (unitExp.Exp + i == Experience.ExpToLevel2)
				{
					Debug.Log($"{unitExp.Exp + i} is equal to xp required to lvl");
					CreateAndExpandPips();
					break;
				}
			}

			pipToFill.GetComponent<Image>().color = pipColors[1];
		}

		Debug.Log($"addExp finished | totalPips: {pips.Count} | emptyPipsQ: {emptyPipQueue.Count}");
	}

	private void CreateAndExpandPips()
	{
		Debug.Log("createExpandPip -----------------------------");
		Debug.Log($"createAndExpandPips started | totalPips: {pips.Count} | emptyPipsQ: {emptyPipQueue.Count}");
		GameObject newPip = pips[0];
		pips.Add(Instantiate(newPip, cont.transform));  //instantiate new pip
		foreach (GameObject pip in pips)
		{
			pip.GetComponent<Image>().color = pipColors[0]; //set all pips to blank
			emptyPipQueue.Enqueue(pip);
		}
		Debug.Log($"createAndExpandPips finished | totalPips: {pips.Count} | emptyPipsQ: {emptyPipQueue.Count}");
	}
}
