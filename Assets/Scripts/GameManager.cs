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
	
	public bool resolved;

	private void Start()
	{
		Reset();
	}

	public void ActorDead(GameObject actor)
	{
		resolved = true;
		if (actor.name == "p_side")
		{
			gameOver.SetActive(true);
		}
		else
		{
			Debug.Log("You won!");
			// transition to shop
		}
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
