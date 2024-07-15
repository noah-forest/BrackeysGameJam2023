using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UnitPreview : MonoBehaviour
{
	public List<GameObject> unitPos;

	public Sprite hiddenUnit;

	private GameManager gameManager;

	private void Start()
	{
		gameManager = GameManager.singleton;
	}

	public void FillUnitPos(List<GameObject> enemyUnits)
	{
		int randomHidden = Random.Range(0, enemyUnits.Count+1);

		for(int i = 0; i < unitPos.Count; ++i)
		{
			UnitMasterComponent master = enemyUnits[i].GetComponent<UnitMasterComponent>();
			RevealUnit unitImg = unitPos[i].GetComponent<RevealUnit>();
			unitImg.unit = enemyUnits[i];
			unitImg.currentSprite = master.unitSprite.sprite;
			unitImg.ShowUnit();
		}

		for(int i = 0; i < randomHidden; ++i)
		{
			RevealUnit unitImg = unitPos[i].GetComponent<RevealUnit>();
			unitImg.HideUnit();
		}
	}

	public void RerollEnemyUnits()
	{
		gameManager.CreateEnemyTeam();
	}
}
