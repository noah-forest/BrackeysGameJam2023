using UnityEngine;
using UnityEngine.SceneManagement;


public class GameManager : MonoBehaviour
{
	#region singleton

	public static GameManager singleton;

	private void Awake()
	{
		if (singleton)
		{
			Destroy(this.gameObject);
			return;
		}

		singleton = this;
		DontDestroyOnLoad(this.gameObject);
	}
	#endregion

	public GameObject gameOver;

	public Actor playerActor;
	public Actor enemyActor;
	
	public bool resolved;

	private void Start()
	{
		Reset();
		playerActor.health.died.AddListener(playerDied);
		enemyActor.health.died.AddListener(enemyDied);
	}

	public void playerDied()
	{
		Debug.Log("You won!");
	}

	public void enemyDied()
	{
		gameOver.SetActive(true);
	}

	public void ReloadScene()
	{
		SceneManager.LoadScene(0);
		Reset();
	}

	private void Reset()
	{
		gameOver.SetActive(false);
		resolved = false;
	}
}
