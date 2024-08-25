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
	public Texture2D quickSellCursor;
	public Texture2D quickBuyCursor;

	public Canvas MainMenuCanvas;
	public Canvas shopCanvas;

	public List<AudioClip> mouseSoundFX;
	private AudioSource soundPlayer;

	private bool hovering;
	private bool hoveringGrave;
	public bool hoveringShopSlot;
	public bool hoveringSlot;

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

	private GameManager gameManager;
	
	private void Start()
	{
		gameManager = GameManager.singleton;
	}

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
		}
		
		if (hoveringSlot && gameManager.controlDown)
		{
			SetQuickSellCursor();
		} else if (hoveringShopSlot && gameManager.controlDown)
		{
			SetQuickBuyCursor();
		} else if (hoveringSlot && Input.GetKeyUp(KeyCode.LeftControl) || hoveringShopSlot && Input.GetKeyUp(KeyCode.LeftControl))
		{
			SetHoverDragCursor();
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
		SetCursor(dragCursor);
	}

	public void SetPressedCursor()
	{
		SetCursor(pressedCursor);
	}

	public void SetShovelPressedCursor()
	{
		SetCursor(shovelDigCursor);
	}

	public void SetHoverDragCursor()
	{
		hovering = false;

		SetCursor(hoverDragCursor);
	}

	public void SetShovelCursor()
	{
		hovering = false;
		hoveringGrave = true;

		SetCursor(shovelCursor);
	}

	public void SetHoverCursor()
	{
		hovering = true;

		SetCursor(hoverCursor);
	}

	public void SetTooltipCursor()
	{
		hovering = false;

		SetCursor(tooltipCursor);
	}

	public void SetQuickSellCursor()
	{
		SetCursor(quickSellCursor);
	}

	public void SetQuickBuyCursor()
	{
		SetCursor(quickBuyCursor);
	}

	public void SetToDefaultCursor()
	{
		hovering = false;
		hoveringGrave = false;
		SetCursor(defaultCursor);
	}

	private void SetCursor(Texture2D cursor)
	{
		Vector2 cursorOffset = new Vector2(cursor.width / 2, cursor.height / 2);
		Cursor.SetCursor(cursor, cursorOffset, CursorMode.Auto);
	}
	#endregion
}
