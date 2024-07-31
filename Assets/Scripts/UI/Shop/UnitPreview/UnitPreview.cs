using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UnitPreview : MonoBehaviour
{
	public List<GameObject> unitPos;

	public Sprite hiddenUnit;

	public int rerollCost = 10;

	private GameManager gameManager;
	private BattleManager battleManager;

	private bool firstTime;

	private void Start()
	{
		gameManager = GameManager.singleton;
		battleManager = BattleManager.singleton;

		gameManager.startGame.AddListener(Reset);

		firstTime = true;
	}

	private void Reset()
	{
		firstTime = true;
	}

	public void FillUnitPos(List<GameObject> enemyUnits)
	{
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
		if (!firstTime)
		{
			//how many units to hide
			int randomHidden = Random.Range(0, enemyUnits.Count + 1);

			//roll it twice to lower the chances of getting all 3 hidden
			if (randomHidden == enemyUnits.Count)
			{
				randomHidden = Random.Range(0, enemyUnits.Count + 1);
			}

			for (int i = 0; i < randomHidden; ++i)
			{
				RevealUnit unitImg = unitPos[i].GetComponent<RevealUnit>();
				unitImg.HideUnit();
			}
		}

		firstTime = false;
	}

	public void RerollEnemyUnits()
	{
		battleManager.CreateEnemyTeam();
		gameManager.previewRolled?.Invoke();
	}
}
