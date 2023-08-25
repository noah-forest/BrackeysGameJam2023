using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
	public GameManager gameManager;
	
	private void Start()
	{
		gameManager = GameManager.singleton;
	}

	public void StartClicked()
	{
		gameManager.StartGame();
	}

	public void ExitClicked()
	{
		Application.Quit();
	}
}
