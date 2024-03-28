using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
	[SerializeField] GameObject livesContainer;
	[SerializeField] GameObject livesArea;
	public Sprite emptyHeart;
	public Sprite fullHeart;
	private List<Image> hearts = new();

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
	}

    private void ShowPauseMenu()
    {
        pauseMenuScreen.SetActive(gameManager.openThePauseMenuPleaseGoodSir);
    }

    private void UpdateGoldText()
    {
        goldUI.text = gameManager.Gold.ToString();
    }
    private void UpdateLivesText()
    {
        livesUI.text = gameManager.Lives.ToString();
    }

	public void ShowConfirmUI()
	{
		for(int i = 0; i < gameManager.lanes.Count; i++)
		{
			if (gameManager.playerBattleSlots[i].payload == null)
			{
				confirmUI.SetActive(true);
				break;
			} else if(i >= gameManager.lanes.Count-1 && gameManager.playerBattleSlots[i].payload != null)
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
			textAnim.SetTrigger("won");
			cmpResult.color = (Color)new Color32(159, 255, 97, 255); // set the color to GREEN
			top.SetActive(false);
			livesArea.SetActive(false);
			gameManager.wonParticles.SetActive(true);
		} else if (index == 1) // if the battle is LOST
		{
			textAnim.SetTrigger("lose");
			livesArea.SetActive(true);
			top.SetActive(true);
			cmpResult.color = (Color)new Color32(255, 95, 95, 255); // set the color to RED
		}
	}

	private void LoadLives()
	{
		for (int i = 0; i < hearts.Count; i++)
		{
			if (i < gameManager.Lives)
			{
				hearts[i].sprite = fullHeart;
			}
			else
			{
				hearts[i].sprite = emptyHeart;
			}
		}
	}

	#endregion

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
