using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TabGroup : MonoBehaviour
{
	public List<PatchNoteButton> tabButtons;

	public TextMeshProUGUI content;

	public Color tabIdle;
	public Color tabHover;
	public Color tabActive;

	public PatchNoteButton selectedTab;

	public void Subscribe(PatchNoteButton button)
	{
		if(tabButtons == null)
		{
			tabButtons = new();
		}

		tabButtons.Add(button);
	}

	public void OnTabEnter(PatchNoteButton button)
	{
		ResetTabs();
		if (selectedTab == null || button != selectedTab)
		{
			button.foreground.color = tabHover;
		}
	}

	public void OnTabExit(PatchNoteButton button)
	{
		ResetTabs();
	}

	public void OnTabSelected(PatchNoteButton button)
	{
		selectedTab = button;
		ResetTabs();
		button.foreground.color = tabActive;
		content.text = button.patchNote.text;
		content.text = content.text.Replace("<bullet>", Convert.ToChar(0x2022).ToString());
	}

	public void ResetTabs()
	{
		foreach(PatchNoteButton button in tabButtons)
		{
			if (selectedTab != null && button == selectedTab) continue;
			button.foreground.color = tabIdle;
		}
	}
}
