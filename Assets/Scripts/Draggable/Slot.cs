using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Slot : MonoBehaviour, IPointerEnterHandler, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler, IDragHandler, IEndDragHandler
{
    public UnityEvent<GameObject, GameObject> itemUpdatedEvent;
    public UnityEvent dragStarted;
    public UnityEvent dragStopped;
    public static Slot currentlyOverSlot;
    public GameObject _payload;

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
            itemUpdatedEvent.Invoke(old, value);
            Render();
        }
    }

    private void Start()
    {
        Render();
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

    protected void Render()
    {
        foreach (ISlotRenderer renderer in gameObject.GetComponentsInChildren<ISlotRenderer>())
        {
            renderer.RenderSlot(payload);
        }
    }

    protected void OnMouseDown()
    {
        foreach (ISlotRenderer renderer in gameObject.GetComponentsInChildren<ISlotRenderer>())
        {
            renderer.SlotDragStarted(payload);
        }

        if (_payload != null && spriteDraggingRepresentation == null)
        {
            spriteDraggingRepresentation = CreateSpriteObject();
            spriteDraggingRepresentation.position = Input.mousePosition;
        }
    }

    protected void OnMouseDrag()
    {
        //Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        if (spriteDraggingRepresentation)
        {
            spriteDraggingRepresentation.position = Input.mousePosition;
        }
    }

    protected void OnMouseEnter()
    {
        currentlyOverSlot = this;
    }

    protected void OnMouseExit()
    {
        if (currentlyOverSlot == this)
        {
            currentlyOverSlot = null;
        }
    }

    protected void OnMouseUp()
    {
        foreach (ISlotRenderer renderer in gameObject.GetComponentsInChildren<ISlotRenderer>())
        {
            renderer.SlotDragEnded(payload);
        }

        if (spriteDraggingRepresentation)
        {
            Destroy(spriteDraggingRepresentation.parent.gameObject);
        }

        if (payload != null && currentlyOverSlot != null && currentlyOverSlot != this)
        {
            // Swaps currentlyOverSlot current item and this.payload
            (payload, currentlyOverSlot.payload) = (currentlyOverSlot.payload, payload);
            currentlyOverSlot = null;
        }
        else
        {
            Render();
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
        spriteDraggingRepresentation.position = Input.mousePosition;
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
