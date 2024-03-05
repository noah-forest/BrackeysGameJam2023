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

public class Slot : MonoBehaviour, IPointerEnterHandler, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler, IDragHandler, IEndDragHandler
{
    public static Slot currentlyOverSlot;
    public static DragVisual _dragVisual;

    public UnityEvent dragStarted = new();
    public UnityEvent dragStopped = new();

	public static UnityEvent<Slot> anyDragStarted = new(); 
    public static UnityEvent<Slot> anyDragStopped = new();

    public static DragVisual dragVisual
    {
        get
        {
            if (_dragVisual == null)
            {
                _dragVisual = Instantiate(Resources.Load("DraggingVisualObject") as GameObject).GetComponent<DragVisual>();
                Debug.Log("instantited", _dragVisual);
            }

            return _dragVisual;
        }
    }

    private List<SlotDropPrecheck> slotDropPrechecks = new();
    private List<SlotRetrievePrecheck> slotRetrievePrechecks = new();
    
    public GameObject _payload;

    [HideInInspector]
    public MouseUtils mouseUtils;
    
    protected Transform spriteDraggingRepresentation;

    private bool isDragged;

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

        OnPayloadChanged();
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
            isDragged = true;
        }
    }

    protected void OnMouseDrag()
    {
        //Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        dragVisual.MoveTo(Input.mousePosition);
    }

    protected void OnMouseEnter()
    {
        if(!isDragged) mouseUtils.SetHoverDragCursor();
		currentlyOverSlot = this;
    }

    protected void OnMouseExit()
    {
        if(!isDragged) mouseUtils.SetToDefaultCursor();
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
        isDragged = false;

		if(currentlyOverSlot == this)
		{
			mouseUtils.SetHoverDragCursor();
		} else
		{
			mouseUtils.SetToDefaultCursor();
		}

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
				if (this.payload.name == draggedToSlot.payload.name)
				{
					GameObject tempPayload = payload;
					payload = null;
					Destroy(tempPayload);
					Debug.Log("level up unit");
					return;
				}
			}

			(this.payload, draggedToSlot.payload) = (draggedToSlot.payload, payload);
		}
    }

    //To work with UI

    public void OnPointerEnter(PointerEventData data)
    {
        this.OnMouseEnter();
    }

    public void OnPointerExit(PointerEventData data)
    {
        this.OnMouseExit();
    }

    public void OnPointerDown(PointerEventData data)
    {
        this.OnMouseDown();

        //Fixes weird bug where initial position is wrong
        dragVisual.SnapTo(Input.mousePosition);
    }

    public void OnPointerUp(PointerEventData data)
    {
        this.OnMouseUp();
    }

    public void OnDrag(PointerEventData eventData)
    {
        this.OnMouseDrag();
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        this.OnMouseUp();
    }

    
}
