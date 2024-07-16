using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UnitPreview : MonoBehaviour
{
	public List<GameObject> unitPos;

	public Sprite hiddenUnit;

	public int rerollCost = 10;

	private GameManager gameManager;

	private void Start()
	{
		gameManager = GameManager.singleton;
	}

	public void FillUnitPos(List<GameObject> enemyUnits)
	{
		//how many units to hide
		int randomHidden = Random.Range(0, enemyUnits.Count+1);

		//roll it twice to lower the chances of getting all 3 hidden
		if (randomHidden == enemyUnits.Count)
		{
			randomHidden = Random.Range(0, enemyUnits.Count + 1);
		}

		//show units not hidden
		for (int i = 0; i < unitPos.Count; ++i)
		{
			UnitMasterComponent master = enemyUnits[i].GetComponent<UnitMasterComponent>();
			RevealUnit unitImg = unitPos[i].GetComponent<RevealUnit>();
			unitImg.unit = enemyUnits[i];
			unitImg.currentSprite = master.unitSprite.sprite;
			unitImg.ShowUnit();
		}

		//hide the units
		for(int i = 0; i < randomHidden; ++i)
		{
			RevealUnit unitImg = unitPos[i].GetComponent<RevealUnit>();
			unitImg.HideUnit();
		}
	}

	public void RerollEnemyUnits()
	{
		gameManager.CreateEnemyTeam();
		gameManager.previewRolled?.Invoke();
	}
}
