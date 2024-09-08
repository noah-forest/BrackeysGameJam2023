using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class LB_Tab : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
	public bool firstSelected;
	public LB_TabManager manager;
	public Image background;

	private MouseUtils mouseUtils;

	private void Start()
	{
		
		mouseUtils = MouseUtils.singleton;
		manager.Subscribe(this);
		if(firstSelected) manager.OnTabSelected(this);
	}

	public void OnPointerClick(PointerEventData eventData)
	{
		manager.OnTabSelected(this);
	}

	public void OnPointerEnter(PointerEventData eventData)
	{
		if(mouseUtils) mouseUtils.SetHoverCursor();
		manager.OnTabEnter(this);
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		if(mouseUtils) mouseUtils.SetToDefaultCursor();
		manager.OnTabExit(this);
	}
}
