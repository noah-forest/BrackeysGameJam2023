using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public delegate bool SlotDropPrecheck(Slot oldSlot, Slot newSlot);
public delegate bool SlotRetrievePrecheck(Slot oldSlot, Slot newSlot);

public class Slot : MonoBehaviour, IPointerEnterHandler, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler, IDragHandler, IEndDragHandler, IPointerClickHandler
{
    public static Slot currentlyOverSlot;
    public static DragVisual _dragVisual;

    public UnityEvent dragStarted = new();
    public UnityEvent dragStopped = new();

	public static UnityEvent<Slot> controlClicked = new();
	public static UnityEvent<Slot> anyDragStarted = new(); 
    public static UnityEvent<Slot> anyDragStopped = new();

	public bool interactable = true;

	private bool beingDragged;

	private bool controlDown;
	
    public static DragVisual dragVisual
    {
        get
        {
            if (_dragVisual == null)
            {
                _dragVisual = Instantiate(Resources.Load("DraggingVisualObject") as GameObject).GetComponent<DragVisual>();
            }

            return _dragVisual;
        }
    }

    private List<SlotDropPrecheck> slotDropPrechecks = new();
    private List<SlotRetrievePrecheck> slotRetrievePrechecks = new();
    
    public GameObject _payload;

    [HideInInspector]
    public MouseUtils mouseUtils;

	[HideInInspector]
	public GameManager gameManager;
    
    protected Transform spriteDraggingRepresentation;

    public GameObject payload
    {
        get => _payload;
        set
        {
            if (value != null && value.GetComponent<ISlotItem>() == null)
            {
                Debug.LogWarning("No ISlotItem on payload", gameObject);
            }
            
            GameObject old = this.payload;
            this._payload = value;
            OnPayloadChanged();
		}
    }

    private void Start()
    {
		mouseUtils = MouseUtils.singleton;
		gameManager = GameManager.singleton;

        OnPayloadChanged();
    }

    private void Update()
    {
	    controlDown = Input.GetKey(KeyCode.LeftControl);
    }

    public void AddDropPrecheck(SlotDropPrecheck precheck)
    {
        slotDropPrechecks.Add(precheck);
    }
    
    public void AddRetrievePrecheck(SlotRetrievePrecheck precheck)
    {
        slotRetrievePrechecks.Add(precheck);
    }

    public Transform CreateSpriteObject()
    {
        GameObject canvasObj = new GameObject("drag", typeof(RectTransform), typeof(Canvas));
        GameObject obj = new GameObject("drag", typeof(RectTransform), typeof(Image));
        Image img = obj.GetComponent<Image>();
        img.preserveAspect = true;
        obj.transform.SetParent(canvasObj.transform);
        Canvas canvas = canvasObj.GetComponent<Canvas>();
        img.sprite = GetSpriteForDragging();
        canvas.overrideSorting = true;
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 10000;
        return obj.transform;
    }

    public Sprite GetSpriteForDragging()
    {
        ISlotItem renderer = payload.GetComponent<ISlotItem>(); 
        return renderer?.GetSlotSprite();
    }

    protected void OnPayloadChanged()
    {
		foreach (ISlotPayloadChangeHandler renderer in gameObject.GetComponentsInChildren<ISlotPayloadChangeHandler>())
        {
            renderer.SlotPayloadChanged(payload);
        }
		if(gameManager != null) gameManager.unitAddedToSlot?.Invoke(this);
	}

    protected void OnMouseDown()
    {
        foreach (ISlotStartDragHandler renderer in gameObject.GetComponentsInChildren<ISlotStartDragHandler>())
        {
            renderer.SlotDragStarted(payload);
        }

        if (_payload != null && spriteDraggingRepresentation == null)
        {
            dragVisual.SetSprite(GetSpriteForDragging());
            dragVisual.SnapTo(Input.mousePosition);
            dragVisual.DragStarted();
            dragStarted.Invoke();
            anyDragStarted.Invoke(this);
			mouseUtils.SetDragCursor();
		}
    }

    protected void OnMouseDrag()
    {
        //Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        dragVisual.MoveTo(Input.mousePosition);
		if (payload != null)
		{
			beingDragged = true;
			mouseUtils.SetDragCursor();
		}
	}

    protected void OnMouseEnter()
    {
		if (payload != null) mouseUtils.SetHoverDragCursor();
		currentlyOverSlot = this;
    }

    protected void OnMouseExit()
    {
		mouseUtils.SetToDefaultCursor();
        if (currentlyOverSlot == this)
        {
            currentlyOverSlot = null;
        }
    }

    protected void OnMouseUp()
    {
        foreach (ISlotEndDragHandler renderer in gameObject.GetComponentsInChildren<ISlotEndDragHandler>())
        {
            renderer.SlotDragEnded(payload);
        }

        dragVisual.DragStopped();
        dragStopped.Invoke();
        anyDragStopped.Invoke(this);
        beingDragged = false;

		if(payload != null) mouseUtils.SetHoverDragCursor();

		if (payload != null && currentlyOverSlot != null && currentlyOverSlot != this)
        {
			SwapSlots(currentlyOverSlot);
            currentlyOverSlot = null;
        }
        else
        {
			OnPayloadChanged();
		}
    }

    private void SwapSlots(Slot draggedToSlot)
    {
        bool oldSlotPassed = slotRetrievePrechecks.TrueForAll(check => check(this, draggedToSlot));
        bool draggedToSlotPassed = draggedToSlot.slotDropPrechecks.TrueForAll(check => check(this, draggedToSlot));

		if (draggedToSlotPassed && oldSlotPassed)
		{
			if (payload && draggedToSlot.payload) 
			{
				// check if ur dragging onto the same unit
				if (this.payload.name == draggedToSlot.payload.name)
				{
					GameObject tempPayload = payload;

					// grab the experience from both units
					Experience incUnitExp = payload.GetComponent<Experience>();
					Experience targetUnitExp = draggedToSlot.payload.GetComponent<Experience>();

					// if either of them are lvl 3, do nothing
					// todo: revert to switch
					if (incUnitExp.curLevel == Experience.MaxLevel || targetUnitExp.curLevel == Experience.MaxLevel) return;

					payload = null;
					Destroy(tempPayload);

					//Level up unit
					int extraXp = 0;
					if(incUnitExp.curLevel == 2)
					{
						extraXp = Experience.ExpToLevel2;
					}

					targetUnitExp.AddExp(incUnitExp.Exp + 1 + extraXp);

					//update the tooltip as soon as a unit levels up
					SlotTooltipTrigger tooltip = draggedToSlot.GetComponent<SlotTooltipTrigger>();
					tooltip.ShowTooltip(draggedToSlot);

					return;
				}
			}
			(payload, draggedToSlot.payload) = (draggedToSlot.payload, payload);
		}
    }

    protected void OnMouseClick()
    {
	    if (!controlDown) return;
	    controlClicked.Invoke(this);
    }

    //To work with UI

    public void OnPointerEnter(PointerEventData data)
    {
		if (interactable)
		{
			this.OnMouseEnter();
		}
    }

    public void OnPointerExit(PointerEventData data)
    {
		if (interactable)
		{
			this.OnMouseExit();
		}
	}

    public void OnPointerDown(PointerEventData data)
    {
		if (interactable)
		{
			this.OnMouseDown();

			//Fixes weird bug where initial position is wrong
			dragVisual.SnapTo(Input.mousePosition);
		}
    }

    public void OnPointerUp(PointerEventData data)
    {
		if (interactable)
		{
			this.OnMouseUp();
		}
    }

    public void OnDrag(PointerEventData eventData)
    {
		if (interactable)
		{
			this.OnMouseDrag();
		}
    }

    public void OnEndDrag(PointerEventData eventData)
    {
		if (interactable)
		{
			this.OnMouseUp();
		}
    }

    public void OnPointerClick(PointerEventData eventData)
    {
	    if (interactable)
	    {
		    this.OnMouseClick();
	    }
    }
}
