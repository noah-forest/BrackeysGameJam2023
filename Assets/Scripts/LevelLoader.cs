using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelLoader : MonoBehaviour
{
	public Animator transition;
	public float transitionTime = 1.0f;

	private GameManager gameManager;

	private void Start()
	{
		gameManager = GameManager.singleton;

		gameManager.startGame.AddListener(StartGame);
		gameManager.startBattle.AddListener(StartBattle);
		gameManager.loadUI.AddListener(LoadUI);
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

	IEnumerator LoadLevel(string levelName)
	{
		transition.SetTrigger("start");

		yield return new WaitForSeconds(transitionTime);

		SceneManager.LoadScene(levelName);
	}

	IEnumerator WaitToLoad()
	{
		yield return new WaitForSeconds(transitionTime);

		gameManager.loadUI.Invoke();
	}
}
