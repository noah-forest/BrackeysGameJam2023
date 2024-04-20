using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PostProcessing : MonoBehaviour
{
	public GameObject volume;

	private void OnEnable()
	{
		Settings.postProcessingToggled.AddListener(PpToggled);
	}

	private void OnDisable()
	{
		Settings.postProcessingToggled.RemoveListener(PpToggled);
	}

	private void PpToggled()
	{
		volume.SetActive(Settings.PostProcessing);
	}
}
