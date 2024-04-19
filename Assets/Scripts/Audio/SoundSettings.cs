using System;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Events;
using UnityEngine.UI;

public class SoundSettings : MonoBehaviour
{
	public UnityEvent sliderValueChanged;

	public Slider soundSlider;

	private float _sliderValue;

    public float SliderValue
	{
		get => _sliderValue;
		set
		{
			_sliderValue = value;
			sliderValueChanged.Invoke();
		}
	}

	[SerializeField] private AudioMixer masterMixer;

    private void Start()
    {
        SetVolume(PlayerPrefs.GetFloat("SavedMasterVolume", soundSlider.value));
		SliderValue = soundSlider.value;
	}

    public void SetVolume(float _value)
    {
        if (_value < 1)
        {
            _value = .001f;
        }
        
        RefreshSlider(_value);
        PlayerPrefs.SetFloat("SavedMasterVolume", _value);
        masterMixer.SetFloat("MasterVolume", Mathf.Log10(_value / 100) * 20f);
    }
    
    public void SetVolumeFromSlider()
    {
        SetVolume(soundSlider.value);
		SliderValue = soundSlider.value;
	}

    private void RefreshSlider(float _value)
    {
		soundSlider.value = _value;
		SliderValue = soundSlider.value;
	}
}