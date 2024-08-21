using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PatchNoteButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
	public bool firstSelected;

	public TabGroup tabGroup;

	public TextAsset patchNote;
	public Image foreground;

	private MouseUtils mouseUtils;

	public void OnPointerClick(PointerEventData eventData)
	{
		tabGroup.OnTabSelected(this);
	}

	public void OnPointerEnter(PointerEventData eventData)
	{
		mouseUtils.SetHoverCursor();
		tabGroup.OnTabEnter(this);
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		mouseUtils.SetToDefaultCursor();
		tabGroup.OnTabExit(this);
	}

	private void Start()
	{
		mouseUtils = MouseUtils.singleton;

		tabGroup.Subscribe(this);
		if (firstSelected)
		{
			tabGroup.OnTabSelected(this);
		}
	}
}
