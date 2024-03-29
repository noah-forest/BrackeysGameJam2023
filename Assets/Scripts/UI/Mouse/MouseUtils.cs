using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MouseUtils : MonoBehaviour
{
	public Texture2D defaultCursor;
	public Texture2D hoverCursor;
	public Texture2D dragCursor;
	public Texture2D hoverDragCursor;
	public Texture2D shovelCursor;

	public Canvas MainMenuCanvas;
	public Canvas shopCanvas;

	#region singleton

	public static MouseUtils singleton;

	private void Awake()
	{
		if (singleton)
		{
			Destroy(this.gameObject);
			return;
		}

		singleton = this;
		DontDestroyOnLoad(this.gameObject);
		
		SetToDefaultCursor();
	}

	#endregion

	private List<Button> buttons = new();
	private List<Slot> slots = new();
	private EventTrigger trigger;

	public void FindButtonsInScene()
	{
		if(buttons.Count > 0)
		{
			buttons.Clear();
			buttons.TrimExcess();
		}

		GameObject[] buttonsInScene = GameObject.FindGameObjectsWithTag("Button");
		foreach (GameObject button in buttonsInScene)
		{
			if (!button.activeInHierarchy) return;
			buttons.Add(button.GetComponent<Button>());
		}

		EventTrigger.Entry entry = new()
		{
			eventID = EventTriggerType.PointerEnter
		};
		entry.callback.AddListener(functionToCall => SetHoverCursor());

		EventTrigger.Entry exit = new()
		{
			eventID = EventTriggerType.PointerExit
		};
		exit.callback.AddListener(functionToCall => SetToDefaultCursor());

		foreach (Button button in buttons)
		{
			trigger = button.AddComponent<EventTrigger>();
			trigger.triggers.Add(entry);
			trigger.triggers.Add(exit);
		}
	}

	#region Set Mouse Cursors
	public void SetDragCursor()
	{
		// set drag cursor
		Vector2 cursorOffset = new Vector2(dragCursor.width / 2, dragCursor.height / 2);
		Cursor.SetCursor(dragCursor, cursorOffset, CursorMode.Auto);
	}

	public void SetHoverDragCursor()
	{
		Vector2 cursorOffset = new Vector2(hoverDragCursor.width / 2, hoverDragCursor.height / 2);
		Cursor.SetCursor(hoverDragCursor, cursorOffset, CursorMode.Auto);
	}

	public void SetShovelCursor()
	{
		Vector2 cursorOffset = new Vector2(shovelCursor.width / 2, shovelCursor.height / 2);
		Cursor.SetCursor(shovelCursor, cursorOffset, CursorMode.Auto);
	}

	public void SetHoverCursor()
	{
		Vector2 cursorOffset = new Vector2(hoverCursor.width / 2, hoverCursor.height / 2);
		Cursor.SetCursor(hoverCursor, cursorOffset, CursorMode.Auto);
	}

	public void SetToDefaultCursor()
	{
		Vector2 cursorOffset = new Vector2(defaultCursor.width / 2, defaultCursor.height / 2);
		Cursor.SetCursor(defaultCursor, cursorOffset, CursorMode.Auto);
	}
	#endregion
}
