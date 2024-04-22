using UnityEngine;

public class ShowConcede : MonoBehaviour
{
	public GameObject concedeButton;

	private GameManager gameManager;

	private void Start()
	{
		gameManager = GameManager.singleton;

		gameManager.battleStartedEvent.AddListener(Show);
		gameManager.battleEndedEvent.AddListener(Hide);
	}

	private void Show()
	{
		concedeButton.SetActive(true);
	}

	private void Hide()
	{
		concedeButton.SetActive(false);
	}
}
