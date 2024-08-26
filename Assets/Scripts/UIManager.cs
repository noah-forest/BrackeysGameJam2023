using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
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

	[SerializeField] TextMeshProUGUI battleReward;
	[SerializeField] TextMeshProUGUI interestGained;

	[SerializeField] GameObject confirmUI;

	[SerializeField] GameObject gameOverScreen;
	[SerializeField] GameObject pauseMenuScreen;
	[SerializeField] GameObject reserveSlots;

	[SerializeField] GameObject shopUi;
	[SerializeField] GameObject unitPreview;
	[SerializeField] GameObject HUD;

	[SerializeField] GameObject battlesWon;
	[SerializeField] TextMeshProUGUI battlesWonCount;
	[SerializeField] TextMeshProUGUI shopBattlesWonCount;
	[SerializeField] TextMeshProUGUI scaleText;
	[SerializeField] TextMeshProUGUI shopScaleText;
	[SerializeField] TextMeshProUGUI scaleContent;
	[SerializeField] GameObject scaledUI;

	[SerializeField] private GameObject dpsPreview;
	
	[SerializeField] GameObject livesContainer;
	[SerializeField] GameObject livesArea;
	[SerializeField] Sprite fullHeart;
	[SerializeField] Material heartMat;
	private List<Image> hearts = new();
	private Animator heartAnim;

	private float dissolveTime = 2f;

	private int dissolveAmount = Shader.PropertyToID("_DissolveAmount");

	private FadeMusic fadeMusic;
	private GameManager gameManager;
	private BattleManager battleManager;

	private int scaleCountdown;
	private int scaleLevel;
	
	private void Start()
	{
		gameManager = GameManager.singleton;
		battleManager = BattleManager.singleton;
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
		gameManager.startGame.AddListener(ResetBattlesWon);
		UpdateGoldText();
		UpdateLivesText();

		fadeMusic = gameManager.MusicPlayer.GetComponent<FadeMusic>();

		shopBattlesWonCount.text = gameManager.BattlesWon.ToString();
		scaleLevel = battleManager.scaleToLvl2;
		scaleCountdown = scaleLevel - gameManager.BattlesWon;

		shopScaleText.text = scaleCountdown.ToString();

		foreach (Transform child in livesContainer.transform.GetComponentsInChildren<Transform>())
		{
			if (child.name != "container")
			{
				hearts.Add(child.gameObject.GetComponent<Image>());
			}
		}

		ResetHearts();
	}

	private void ResetBattlesWon()
	{
		shopBattlesWonCount.text = "0";
	}

	private void ShowPauseMenu()
	{
		pauseMenuScreen.SetActive(gameManager.openPauseMenu);

		SoundSettings soundSettings = pauseMenuScreen.transform.parent.GetComponent<SoundSettings>();
		soundSettings.RefreshSlider(PlayerPrefs.GetFloat("SavedMasterVolume"));
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
		fadeMusic.FadeOutMusic();
		for (int i = 0; i < battleManager.lanes.Count; i++)
		{
			if (battleManager.playerBattleSlots[i].payload == null)
			{
				confirmUI.SetActive(true);
				break;
			}
			else if (i >= battleManager.lanes.Count - 1 && battleManager.playerBattleSlots[i].payload != null)
			{
				battleManager.BattleTransition();
				break;
			}
		}
	}

	public void CloseConfirmUI()
	{
		fadeMusic.FadeInMusic();
		confirmUI.SetActive(false);
	}

	#region BattleOver
	//this is kind of fucked but its fine.
	private void ShowBattleWonScreen()
	{
		battleOverScreen.SetActive(true);
		fadeMusic.FadeOutMusic();
		battlesWonCount.text = gameManager.BattlesWon.ToString();
		shopBattlesWonCount.text = gameManager.BattlesWon.ToString();

		// countdown the amt of battles till enemies scale
		if (gameManager.BattlesWon == battleManager.scaleToLvl2)    // lvl1s and 2s
		{
			scaleLevel = battleManager.scaleToLvl3;
			scaledUI.SetActive(true);
			scaleContent.text = $"Next scale in:";
			scaleText.text = $"{scaleLevel - gameManager.BattlesWon}";
		}
		else if (gameManager.BattlesWon == battleManager.scaleToLvl3)   // lvl2 and 3s
		{
			scaleLevel = battleManager.scaleToFinal;
			scaledUI.SetActive(true);
			scaleContent.text = $"Next scale in:";
			scaleText.text = $"{scaleLevel - gameManager.BattlesWon}";
		}
		else if (gameManager.BattlesWon >= battleManager.scaleToFinal) // lvl3s only
		{
			scaleCountdown = 0;
			scaledUI.SetActive(true);
			scaleContent.transform.parent.parent.gameObject.SetActive(false);
		}
		else
		{
			// this is because it will always be 1 ahead, due to the way the events happen
			scaleText.text = $"{scaleCountdown - 1}";
			scaledUI.SetActive(false);
			scaleContent.text = "Enemy scales in:";
		}

		// if it isnt at the final stage (the only time its 0)
		if(scaleCountdown != 0)
		{
			scaleCountdown = scaleLevel - gameManager.BattlesWon;
		}

		shopScaleText.text = $"{scaleCountdown}";

		ShowResult(0);
	}

	private void ShowBattleLostScreen()
	{
		battleOverScreen.SetActive(true);
		fadeMusic.FadeOutMusic();
		ShowResult(1);
		LoadLives();
	}

	private void ShowResult(int index)
	{
		battleReward.text = $"{gameManager.settings.battleReward}";
		interestGained.text = $"{battleManager.GainInterest()}";

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
		unitPreview.SetActive(false);
		HUD.SetActive(false);
	}

	private void ShowShop()
	{
		shopUi.SetActive(true);
		unitPreview.SetActive(true);
		reserveSlots.SetActive(true);
		dpsPreview.SetActive(false);
		if(scaleCountdown == 0)
		{
			scaledUI.transform.parent.gameObject.SetActive(false);
		}
	}

	private void HideShop()
	{
		shopUi.SetActive(false);
		unitPreview.SetActive(false);
		reserveSlots.SetActive(false);
		dpsPreview.SetActive(true);
	}

	private void ShowGameOverScreen()
	{
		gameOverScreen.SetActive(true);
		fadeMusic.FadeOutMusic();
	}

	private void HideGameOverScreen()
	{
		gameOverScreen.SetActive(false);
		fadeMusic.FadeInMusic();
	}
}
