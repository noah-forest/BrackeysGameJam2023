using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MouseUtils : MonoBehaviour
{
	public Texture2D defaultCursor;
	public Texture2D hoverCursor;
	public Texture2D dragCursor;

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
    
	private Button[] buttons;
	private EventTrigger trigger;

	private MouseUtils mouseUtils;

	// Start is called before the first frame update
	void Start()
	{
		mouseUtils = MouseUtils.singleton;
        FindButtonsInScene();
	}

	private void FindButtonsInScene()
	{
		buttons = FindObjectsOfType<Button>();
		
		EventTrigger.Entry entry = new EventTrigger.Entry();
		entry.eventID = EventTriggerType.PointerEnter;
		entry.callback.AddListener(functionToCall => SetHoverCursor());
		
		EventTrigger.Entry exit = new EventTrigger.Entry();
		exit.eventID = EventTriggerType.PointerExit;
		exit.callback.AddListener(functionToCall => SetToDefaultCursor());

		for (int i = 0; i < buttons.Length; i++)
		{
			trigger = buttons[i].AddComponent<EventTrigger>();
			trigger.triggers.Add(entry);
			trigger.triggers.Add(exit);
		}
	}
	
	public void OnMouseDrag(Texture2D dragCursor)
	{
		// set drag cursor
		Vector2 cursorOffset = new Vector2(dragCursor.width / 2, dragCursor.height / 2);
		Cursor.SetCursor(dragCursor, cursorOffset, CursorMode.Auto);
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
}
