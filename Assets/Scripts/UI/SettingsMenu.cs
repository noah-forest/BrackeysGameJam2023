using System.Collections.Generic;
using UnityEngine;

public class SettingsMenu : MonoBehaviour
{
	public Settings settings;

	public List<ToggleSwitch> toggles;

	private void OnEnable()
	{
		toggles[0].onToggleOn.AddListener(SimplifyStats);
		toggles[1].onToggleOn.AddListener(RarityBorders);
		toggles[2].onToggleOn.AddListener(PostProcessing);
		toggles[3].onToggleOn.AddListener(AutoCombine);

		toggles[0].onToggleOff.AddListener(SimplifyStats);
		toggles[1].onToggleOff.AddListener(RarityBorders);
		toggles[2].onToggleOff.AddListener(PostProcessing);
		toggles[3].onToggleOff.AddListener(AutoCombine);

		toggles[0].SetToggleManual(Settings.SimplifiedStats);
		toggles[1].SetToggleManual(Settings.RarityBorders);
		toggles[2].SetToggleManual(Settings.PostProcessing);
		toggles[3].SetToggleManual(Settings.AutoCombine);
	}

	private void OnDisable()
	{
		toggles[0].onToggleOn.RemoveListener(SimplifyStats);
		toggles[1].onToggleOn.RemoveListener(RarityBorders);
		toggles[2].onToggleOn.RemoveListener(PostProcessing);
		toggles[3].onToggleOn.RemoveListener(AutoCombine);

		toggles[0].onToggleOff.RemoveListener(SimplifyStats);
		toggles[1].onToggleOff.RemoveListener(RarityBorders);
		toggles[2].onToggleOff.RemoveListener(PostProcessing);
		toggles[3].onToggleOff.RemoveListener(AutoCombine);
	}

	private void SimplifyStats()
	{
		Settings.SimplifiedStats = !Settings.SimplifiedStats;
	}

	private void RarityBorders()
	{
		Settings.RarityBorders = !Settings.RarityBorders;
	}

	private void PostProcessing()
	{
		Settings.PostProcessing = !Settings.PostProcessing;
	}

	private void AutoCombine()
	{
		Settings.AutoCombine = !Settings.AutoCombine;
	}
}
