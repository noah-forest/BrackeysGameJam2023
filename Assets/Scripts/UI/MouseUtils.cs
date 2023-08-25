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
	}
    
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
}
