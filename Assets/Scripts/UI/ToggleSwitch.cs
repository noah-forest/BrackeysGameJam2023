using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ToggleSwitch : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
	[Header("Slider Setup")]
	[SerializeField, Range(0, 1f)] private float sliderValue;
	public Image background;

	public bool CurrentValue { get; private set; }

	private Slider slider;

	[Header("Animation")]
	[SerializeField, Range(0, 1f)] private float animationDuration = 0.5f;
	[SerializeField] private AnimationCurve slideEase = AnimationCurve.EaseInOut(0, 0, 1, 1);

	private Coroutine animateSliderCoroutine;

	[HideInInspector] public UnityEvent onToggleOn;
	[HideInInspector] public UnityEvent onToggleOff;

	private ToggleGroup toggleGroup;

	private MouseUtils mouseUtils;

	private void Awake()
	{
		SetUpToggleComponents();
		mouseUtils = MouseUtils.singleton;
	}

	protected void OnValidate()
	{
		SetUpToggleComponents();
		slider.value = sliderValue;
	}

	private void SetUpToggleComponents()
	{
		if (slider != null) return;
		
		SetUpSliderComponent();
	}

	private void SetUpSliderComponent()
	{
		slider = GetComponent<Slider>();

		if (slider == null)
		{
			Debug.Log("No slider found!", this);
			return;
		}

		slider.interactable = false;

		var sliderColors = slider.colors;
		sliderColors.disabledColor = Color.white;
		slider.colors = sliderColors;
		background.color = new Color32(255, 136, 136, 255);
		slider.transition = Selectable.Transition.None;
	}

	public void SetUpForManager(ToggleGroup manager)
	{
		toggleGroup = manager;
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

		if(animateSliderCoroutine != null)
		{
			StopCoroutine(animateSliderCoroutine);
		}

		animateSliderCoroutine = StartCoroutine(AnimateSlider());
	}

	private IEnumerator AnimateSlider()
	{
		float startValue = slider.value;
		float endValue = CurrentValue ? 1 : 0;

		float time = 0;
		if(animationDuration > 0)
		{
			while(time < animationDuration)
			{
				time += Time.unscaledDeltaTime;
				
				float lerpFactor = slideEase.Evaluate(time / animationDuration);
				slider.value = sliderValue = Mathf.Lerp(startValue, endValue, lerpFactor);

				yield return null;
			}
		}

		slider.value = endValue;
	}

	public void OnPointerClick(PointerEventData eventData)
	{
		Toggle();
	}

	public void OnPointerEnter(PointerEventData eventData)
	{
		mouseUtils.SetHoverCursor();
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		mouseUtils.SetToDefaultCursor();
	}
}
