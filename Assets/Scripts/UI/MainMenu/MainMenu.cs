using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
	[HideInInspector]
	public GameManager gameManager;

	[HideInInspector]
	public MouseUtils mouseUtils;

	private void Start()
	{
		gameManager = GameManager.singleton;
		mouseUtils = MouseUtils.singleton;

		mouseUtils.FindButtonsInScene();
		if (gameManager.MusicPlayer.isPlaying)
		{
			gameManager.MusicPlayer.Stop();
		}
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
