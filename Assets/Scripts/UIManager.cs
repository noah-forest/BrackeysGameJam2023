using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI livesUI;
    [SerializeField] TextMeshProUGUI goldUI;
    [SerializeField] GameObject battleOverScreen;
    [SerializeField] TextMeshProUGUI battleOutcomeText;
    [SerializeField] GameObject shopButton;
    [SerializeField] GameObject nextBattleButton;
    [SerializeField] GameObject gameOverScreen;
    [SerializeField] TextMeshProUGUI livesStatusText;
    [SerializeField] GameObject pauseMenuScreen;
    public List<GameObject> unitBars = new();
    
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
        shopButton.GetComponent<Button>().onClick.AddListener(HideBattleOverScreen);
        nextBattleButton.GetComponent<Button>().onClick.AddListener(HideBattleOverScreen);
        gameManager.shopTransitionEvent.AddListener(ShowShop);
        gameManager.battleStartedEvent.AddListener(HideShop);
        gameManager.pauseGame.AddListener(ShowPauseMenu);
        gameManager.resumeGame.AddListener(ShowPauseMenu);
        gameManager.shopTransitionEvent.AddListener(HideGameOverScreen);
        UpdateGoldText();
        UpdateLivesText();
    }
    private void HideBattleOverScreen()
    {
        battleOverScreen.SetActive(false);
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

    #region BattleOver
    //this is kind of fucked but its fine.
    private void ShowBattleWonScreen() 
    {
        battleOutcomeText.text = "Battle Won";
        battleOverScreen.SetActive(true);
        LoadStatusText();
        LoadContinueButton();
    }

    private void ShowBattleLostScreen()
    {
        battleOutcomeText.text = "Battle Lost";
        battleOverScreen.SetActive(true);
        LoadStatusText();
        LoadContinueButton();
    }
    #endregion
    private void LoadContinueButton()
    {
        if (gameManager.amountOfBattlesCur >= gameManager.amountOfBattlesBeforeShop)
        {
            nextBattleButton.SetActive(false);
            shopButton.SetActive(true);
        }
        else
        {
            nextBattleButton.SetActive(true);
            shopButton.SetActive(false);
        }
    }


    private void ShowShop()
    {
        shopUi.SetActive(true);
    }
    private void HideShop()
    {
        shopUi.SetActive(false);
    }

    private void LoadStatusText()
    {
        livesStatusText.text = $"Lives Remaining: {gameManager.Lives}";
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
