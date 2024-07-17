using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeMusic : MonoBehaviour
{
	private GameManager gameManager;

	private AudioSource musicPlayer;

	private float fadeTime = 4f;
	private float fadeVolume = 0.05f;
	private float originalVolume;

	private void Start()
	{
		gameManager = GameManager.singleton;
		musicPlayer = GetComponent<AudioSource>();

		originalVolume = musicPlayer.volume;

		gameManager.transitionPlaying.AddListener(FadeAudio);
	}

	private void FadeAudio(bool transitioning)
	{
		if (transitioning)
		{
			FadeOutMusic();
		} else
		{
			FadeInMusic();
		}
	}

	public void FadeOutMusic()
	{
		if (musicPlayer.volume == fadeVolume) return;
		musicPlayer.volume = Mathf.Lerp(musicPlayer.volume, fadeVolume, fadeTime);
	}

	public void FadeInMusic()
	{
		musicPlayer.volume = Mathf.Lerp(fadeVolume, originalVolume, fadeTime);
	}
}
