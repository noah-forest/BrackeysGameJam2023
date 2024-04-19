using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AudioButton : MonoBehaviour
{
	public SoundSettings soundSettings;

	[SerializeField] private GameObject sliderObj;
	[SerializeField] private Image currentImage;

	public List<Sprite> audioLevels = new();

	public bool open;

	private void OnEnable()
	{
		ChangeVolumeIndicator();
		soundSettings.sliderValueChanged.AddListener(VolumeChanged);
	}

	private void OnDisable()
	{
		soundSettings.sliderValueChanged.RemoveListener(VolumeChanged);
	}

	private void VolumeChanged()
	{
		ChangeVolumeIndicator();
    }

	private void ChangeVolumeIndicator()
	{
		if (soundSettings.SliderValue == 0)
		{
			//muted
			currentImage.sprite = audioLevels[0];
		}
		else if (soundSettings.SliderValue > 75)
		{
			currentImage.sprite = audioLevels[1];
		}
		else if (soundSettings.SliderValue >= 25)
		{
			currentImage.sprite = audioLevels[2];
		}
		else
		{
			currentImage.sprite = audioLevels[3];
		}
	}

	public void OnClick()
	{
		if (open)
		{
			open = false;
			sliderObj.SetActive(false);
		}
		else
		{
			sliderObj.SetActive(true);
			open = true;
		}
	}
}
