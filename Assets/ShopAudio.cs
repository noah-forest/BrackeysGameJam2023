using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopAudio : MonoBehaviour
{
	public AudioClip[] audioClips;
	public AudioSource audioPlayer;
	
	public void PlayAudioClipOnce(AudioClip audioClip)
	{
		audioPlayer.PlayOneShot(audioClip);
	}
}
