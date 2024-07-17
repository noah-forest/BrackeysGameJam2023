using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class MouseUtils : MonoBehaviour
{
	public Texture2D defaultCursor;
	public Texture2D tooltipCursor;
	public Texture2D hoverCursor;
	public Texture2D pressedCursor;
	public Texture2D dragCursor;
	public Texture2D hoverDragCursor;
	public Texture2D shovelCursor;
	public Texture2D shovelDigCursor;

	public Canvas MainMenuCanvas;
	public Canvas shopCanvas;

	public List<AudioClip> mouseSoundFX;
	private AudioSource soundPlayer;

	private bool hovering;
	private bool hoveringGrave;

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
		soundPlayer = GetComponent<AudioSource>();
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

	private void Update()
	{
		if (hovering)
		{
			if (Input.GetMouseButtonDown(0))
			{
				SetPressedCursor();
				soundPlayer.pitch = Eerp(0.8f, 1.2f, Random.value);
				soundPlayer.PlayOneShot(mouseSoundFX[0]);
			}

			if (Input.GetMouseButtonUp(0))
			{
				SetHoverCursor();
			}
		} else if (hoveringGrave)
		{
			if (Input.GetMouseButtonDown(0))
			{
				SetShovelPressedCursor();
			}

			if (Input.GetMouseButtonUp(0))
			{
				SetShovelCursor();
			}
		} else
		{
			return;
		}
	}

	static float Eerp(float a, float b, float t)
	{
		return a * MathF.Exp(t * MathF.Log(b / a));
	}

	#region Set Mouse Cursors
	public void SetDragCursor()
	{
		hovering = false;

		// set drag cursor
		Vector2 cursorOffset = new Vector2(dragCursor.width / 2, dragCursor.height / 2);
		Cursor.SetCursor(dragCursor, cursorOffset, CursorMode.Auto);
	}

	public void SetPressedCursor()
	{
		Vector2 cursorOffset = new Vector2(pressedCursor.width / 2, pressedCursor.height / 2);
		Cursor.SetCursor(pressedCursor, cursorOffset, CursorMode.Auto);
	}

	public void SetShovelPressedCursor()
	{
		Vector2 cursorOffset = new Vector2(shovelDigCursor.width / 2, shovelDigCursor.height / 2);
		Cursor.SetCursor(shovelDigCursor, cursorOffset, CursorMode.Auto);
	}

	public void SetHoverDragCursor()
	{
		hovering = false;

		Vector2 cursorOffset = new Vector2(hoverDragCursor.width / 2, hoverDragCursor.height / 2);
		Cursor.SetCursor(hoverDragCursor, cursorOffset, CursorMode.Auto);
	}

	public void SetShovelCursor()
	{
		hovering = false;
		hoveringGrave = true;

		Vector2 cursorOffset = new Vector2(shovelCursor.width / 2, shovelCursor.height / 2);
		Cursor.SetCursor(shovelCursor, cursorOffset, CursorMode.Auto);
	}

	public void SetHoverCursor()
	{
		hovering = true;

		Vector2 cursorOffset = new Vector2(hoverCursor.width / 2, hoverCursor.height / 2);
		Cursor.SetCursor(hoverCursor, cursorOffset, CursorMode.Auto);
	}

	public void SetTooltipCursor()
	{
		hovering = false;

		Vector2 cursorOffset = new Vector2(tooltipCursor.width / 2, tooltipCursor.height / 2);
		Cursor.SetCursor(tooltipCursor, cursorOffset, CursorMode.Auto);
	}

	public void SetToDefaultCursor()
	{
		hovering = false;
		hoveringGrave = false;

		Vector2 cursorOffset = new Vector2(defaultCursor.width / 2, defaultCursor.height / 2);
		Cursor.SetCursor(defaultCursor, cursorOffset, CursorMode.Auto);
	}
	#endregion
}
