using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
	[SerializeField] TextMeshProUGUI livesUI;
	[SerializeField] TextMeshProUGUI goldUI;

	[SerializeField] GameObject battleOverScreen;
	[SerializeField] GameObject result;
	private Animator textAnim;
	[SerializeField] GameObject resultShadow;
	[SerializeField] List<Sprite> results = new();
	[SerializeField] GameObject top;

	[SerializeField] GameObject confirmUI;

	[SerializeField] GameObject gameOverScreen;
	[SerializeField] GameObject pauseMenuScreen;
	[SerializeField] GameObject reserveSlots;
	GameManager gameManager;
	[SerializeField] GameObject shopUi;
	[SerializeField] GameObject HUD;

	[SerializeField] GameObject battlesWon;
	[SerializeField] TextMeshProUGUI battlesWonCount;

	[SerializeField] GameObject livesContainer;
	[SerializeField] GameObject livesArea;
	[SerializeField] Sprite fullHeart;
	[SerializeField] Material heartMat;
	private List<Image> hearts = new();
	private Animator heartAnim;

	private float dissolveTime = 2f;

	private int dissolveAmount = Shader.PropertyToID("_DissolveAmount");

	private void Start()
	{
		gameManager = GameManager.singleton;
		gameManager.goldChangedEvent.AddListener(UpdateGoldText);
		gameManager.livesChangedEvent.AddListener(UpdateLivesText);
		gameManager.battleWonEvent.AddListener(ShowBattleWonScreen);
		gameManager.battleLostEvent.AddListener(ShowBattleLostScreen);
		gameManager.gameOverEvent.AddListener(ShowGameOverScreen);
		gameManager.loadShopEvent.AddListener(ShowShop);
		gameManager.battleStartedEvent.AddListener(HideShop);
		gameManager.pauseGame.AddListener(ShowPauseMenu);
		gameManager.resumeGame.AddListener(ShowPauseMenu);
		gameManager.loadShopEvent.AddListener(HideGameOverScreen);
		UpdateGoldText();
		UpdateLivesText();

		foreach (Transform child in livesContainer.transform.GetComponentsInChildren<Transform>())
		{
			if (child.name != "container")
			{
				hearts.Add(child.gameObject.GetComponent<Image>());
			}
		}

		ResetHearts();
	}

	private void ShowPauseMenu()
	{
		pauseMenuScreen.SetActive(gameManager.openThePauseMenuPleaseGoodSir);
	}

	private void UpdateGoldText()
	{
		goldUI.text = gameManager.Cash.ToString();
	}
	private void UpdateLivesText()
	{
		livesUI.text = gameManager.Lives.ToString();
	}

	public void ShowConfirmUI()
	{
		for (int i = 0; i < gameManager.lanes.Count; i++)
		{
			if (gameManager.playerBattleSlots[i].payload == null)
			{
				confirmUI.SetActive(true);
				break;
			}
			else if (i >= gameManager.lanes.Count - 1 && gameManager.playerBattleSlots[i].payload != null)
			{
				gameManager.BattleTransition();
				break;
			}
		}
	}

	#region BattleOver
	//this is kind of fucked but its fine.
	private void ShowBattleWonScreen()
	{
		battleOverScreen.SetActive(true);
		battlesWonCount.text = gameManager.BattlesWon.ToString();
		ShowResult(0);
	}

	private void ShowBattleLostScreen()
	{
		battleOverScreen.SetActive(true);
		ShowResult(1);
		LoadLives();
	}

	private void ShowResult(int index)
	{
		Image cmpResult = result.GetComponent<Image>();
		textAnim = result.GetComponent<Animator>();
		Image shadResult = resultShadow.GetComponent<Image>();
		cmpResult.sprite = results[index];
		shadResult.sprite = cmpResult.sprite;

		if (index == 0) // if the battle is WON
		{
			cmpResult.color = (Color)new Color32(159, 255, 97, 255); // set the color to GREEN
			top.SetActive(false);
			livesArea.SetActive(false);
			battlesWon.SetActive(true);
			gameManager.wonParticles.SetActive(true);

			textAnim.SetTrigger("won");
		}
		else if (index == 1) // if the battle is LOST
		{
			cmpResult.color = (Color)new Color32(255, 95, 95, 255); // set the color to RED
			livesArea.SetActive(true);
			battlesWon.SetActive(false);
			top.SetActive(true);

			textAnim.SetTrigger("lose");
		}
	}

	private void LoadLives()
	{
		for (int i = 0; i < hearts.Count; i++)
		{
			if (i >= gameManager.Lives)
			{
				//dissolve heart
				heartAnim = hearts[i].GetComponent<Animator>();
				StartCoroutine(WaitToVanish(hearts[i]));
			}
		}
	}

	private IEnumerator Vanish(Image heartToDissolve)
	{
		float elapsedTime = 0f;
		while (elapsedTime < dissolveTime)
		{
			elapsedTime += Time.unscaledDeltaTime;

			float lerpedDissolve = Mathf.Lerp(0, 1.1f, (elapsedTime / dissolveTime));

			heartToDissolve.material.SetFloat(dissolveAmount, lerpedDissolve);
			yield return null;
		}
		heartToDissolve.color = (Color)new Color32(0, 0, 0, 0);
		heartToDissolve.sprite = null;
	}

	private IEnumerator WaitToVanish(Image heartToDissolve)
	{
		heartAnim.SetTrigger("grow");
		yield return new WaitForSecondsRealtime(0.6f);
		StartCoroutine(Vanish(heartToDissolve));
	}

	public void ResetHearts()
	{
		foreach (Image heart in hearts)
		{
			heart.material.SetFloat(dissolveAmount, 0f);
			heart.color = (Color)new Color32(255, 255, 255, 255);
			heart.gameObject.transform.localScale = Vector3.one;
			heart.sprite = fullHeart;
		}
	}

	#endregion

	public void HideMenus()
	{
		shopUi.SetActive(false);
		HUD.SetActive(false);
	}
	
	private void ShowShop()
	{
		shopUi.SetActive(true);
		reserveSlots.SetActive(true);
	}

	private void HideShop()
	{
		shopUi.SetActive(false);
		reserveSlots.SetActive(false);
	}

	private void ShowGameOverScreen()
	{
		gameOverScreen.SetActive(true);
	}

	private void HideGameOverScreen()
	{
		gameOverScreen.SetActive(false);
	}
}
