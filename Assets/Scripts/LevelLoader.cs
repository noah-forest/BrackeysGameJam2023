using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelLoader : MonoBehaviour
{
	public Animator transition;
	public float transitionTime = 1.0f;

	public bool transitionIsPlaying;

	private GameManager gameManager;

	private void Start()
	{
		transitionIsPlaying = false;

		gameManager = GameManager.singleton;

		gameManager.startGame.AddListener(StartGame);
		gameManager.startBattle.AddListener(StartBattle);
		gameManager.loadUI.AddListener(LoadUI);

		gameManager.startShopTransition.AddListener(LoadShopTrans);
		gameManager.startBattleTransition.AddListener(LoadBattleTrans);
	}

	private void StartGame()
	{
		StartCoroutine(LoadLevel("MainMenu"));
	}

	private void StartBattle()
	{
		StartCoroutine(LoadLevel("battle"));
		StartCoroutine(WaitToLoad());
	}

	private void LoadUI()
	{
		gameManager.LoadShop();
		gameManager.HUD.SetActive(true);
	}

	private void LoadShopTrans()
	{
		StartCoroutine(LoadLevel("transition"));
		StartCoroutine(WaitToTransShop());
		StartCoroutine(LoadLevel("battle"));
	}

	private void LoadBattleTrans()
	{
		StartCoroutine(LoadLevel("transition"));
		StartCoroutine(WaitToTransBattle());
		StartCoroutine(LoadLevel("battle"));
	}

	IEnumerator LoadLevel(string levelName)
	{
		transition.SetTrigger("start");

		transitionIsPlaying = true;

		yield return new WaitForSeconds(transitionTime);

		SceneManager.LoadScene(levelName);

		transitionIsPlaying = false;
	}

	IEnumerator WaitToTransShop()
	{
		if (Time.timeScale != 1)
		{
			Time.timeScale = 1;
		}

		yield return new WaitForSeconds(transitionTime);

		gameManager.LoadShop();
	}

	IEnumerator WaitToTransBattle()
	{
		yield return new WaitForSeconds(transitionTime);

		gameManager.NextBattleButton();
	}

	IEnumerator WaitToLoad()
	{
		yield return new WaitForSeconds(transitionTime);

		gameManager.loadUI.Invoke();
	}
}
