using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextAnimController : MonoBehaviour
{
	[SerializeField]
	private Animator moneyAnimator;
	[SerializeField]
	private Animator livesAnimator;

	private GameManager gameManager;

	private void Start()
	{
		gameManager = GameManager.singleton;
		gameManager.goldChangedEvent.AddListener(MoneyTextChanged);
		gameManager.livesChangedEvent.AddListener(LivesTextChanged);
	}

	private void MoneyTextChanged()
	{
		//play anim
		moneyAnimator.SetTrigger("TextChanged");
	}

	private void LivesTextChanged()
	{
		livesAnimator.SetTrigger("TextChanged");
	}
}
