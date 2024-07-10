using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ToggleSwitch : MonoBehaviour, IPointerClickHandler
{
	[Header("Slider Setup")]
	[SerializeField, Range(0, 1f)] private float sliderValue;
	public Image background;

	public bool CurrentValue { get; private set; }

	private Slider _slider;

	[Header("Animation")]
	[SerializeField, Range(0, 1f)] private float animationDuration = 0.5f;
	[SerializeField] private AnimationCurve slideEase = AnimationCurve.EaseInOut(0, 0, 1, 1);

	private Coroutine _animateSliderCoroutine;

	[HideInInspector] public UnityEvent onToggleOn;
	[HideInInspector] public UnityEvent onToggleOff;

	private ToggleGroup _toggleGroup;

	private void Awake()
	{
		SetUpToggleComponents();
	}

	protected void OnValidate()
	{
		SetUpToggleComponents();
		_slider.value = sliderValue;
	}

	private void SetUpToggleComponents()
	{
		if (_slider != null) return;

		SetUpSliderComponent();
	}

	private void SetUpSliderComponent()
	{
		_slider = GetComponent<Slider>();

		if (_slider == null)
		{
			Debug.Log("No slider found!", this);
			return;
		}

		_slider.interactable = false;

		var sliderColors = _slider.colors;
		sliderColors.disabledColor = Color.white;
		_slider.colors = sliderColors;
		_slider.transition = Selectable.Transition.None;
	}

	public void SetUpForManager(ToggleGroup manager)
	{
		_toggleGroup = manager;
	}

	private void Toggle()
	{
		SetStateAndStartAnimation(!CurrentValue, true);
	}

	public void SetToggleManual(bool state)
	{
		SetStateAndStartAnimation(state, false);
	}

	private void SetStateAndStartAnimation(bool state, bool toggleEvent)
	{
		CurrentValue = state;
		if (CurrentValue)
		{
			if (toggleEvent) onToggleOn?.Invoke();
			background.color = new Color32(112, 183, 110, 255);
		} else
		{
			if (toggleEvent) onToggleOff?.Invoke();
			background.color = new Color32(255, 136, 136, 255);
		}

		if(_animateSliderCoroutine != null)
		{
			StopCoroutine(_animateSliderCoroutine);
		}

		_animateSliderCoroutine = StartCoroutine(AnimateSlider());
	}

	private IEnumerator AnimateSlider()
	{
		float startValue = _slider.value;
		float endValue = CurrentValue ? 1 : 0;

		float time = 0;
		if(animationDuration > 0)
		{
			while(time < animationDuration)
			{
				time += Time.unscaledDeltaTime;
				
				float lerpFactor = slideEase.Evaluate(time / animationDuration);
				_slider.value = sliderValue = Mathf.Lerp(startValue, endValue, lerpFactor);

				yield return null;
			}
		}

		_slider.value = endValue;
	}

	public void OnPointerClick(PointerEventData eventData)
	{
		Toggle();
	}
}
