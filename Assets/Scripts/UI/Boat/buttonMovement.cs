using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.PlayerLoop;
using UnityEngine.UI;

public enum movementType
{
	Left,
	Right
}

public class buttonMovement : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
{
	public BoatController boatController;

	public movementType wayToMove;
	
	private Button button;
	private MouseUtils mouseUtils;
	
	private bool pointerDown;
	private void Start()
	{
		button = GetComponent<Button>();
		mouseUtils = MouseUtils.singleton;
	}

	public void OnPointerEnter(PointerEventData eventData)
	{
		if(mouseUtils) mouseUtils.SetHoverCursor();
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		if(mouseUtils) mouseUtils.SetToDefaultCursor();
	}

	public void OnPointerDown(PointerEventData eventData)
	{
		pointerDown = true;
	}

	public void OnPointerUp(PointerEventData eventData)
	{
		pointerDown = false;
	}

	private void Update()
	{
		if (pointerDown)
		{
			if (wayToMove == movementType.Left)
			{
				boatController.MoveLeft();
			} else if (wayToMove == movementType.Right)
			{
				boatController.MoveRight();
			}
		}
		else
		{
			pointerDown = false;
		}
	}
}
