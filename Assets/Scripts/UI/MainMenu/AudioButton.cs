using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AudioButton : MonoBehaviour
{
	[SerializeField] private Sprite buttonDefault;
	[SerializeField] private Sprite buttonDisabled;
    [SerializeField] private GameObject sliderObj;
	[SerializeField] private Slider slider;

	private Image currentImage;

	public bool open;

	private void Start()
	{
		currentImage = GetComponent<Image>();
	}

	private void Update()
	{
		if(slider.value <= 0.005)
		{
			currentImage.sprite = buttonDisabled;
		} else
		{
			currentImage.sprite = buttonDefault;
		}
	}

	public void OnClick()
    {
		if (open)
		{
			open = false;
			sliderObj.SetActive(false);
		} else
		{
			sliderObj.SetActive(true);
			open = true;
		}
    }
}
