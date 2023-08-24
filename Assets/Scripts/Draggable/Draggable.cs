using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Draggable : MonoBehaviour
{
    public GameObject payload;
    public Sprite spriteForDragging;

    protected Transform currentlyDragging;

    public void Place(Vector3 position, Slot slot)
    {
        transform.position = position;
    }

    public void DragTo(Vector3 mousePosition)
    {
        Vector3 position = Camera.main.ScreenToWorldPoint(mousePosition);
        if (currentlyDragging)
        {
            currentlyDragging.position = mousePosition;// new Vector3(position.x, position.y, 0);
        }
    }

    public void SnapTo(Vector3 position)
    {
        if (currentlyDragging)
        {
            currentlyDragging.position = position;
        }
    }

    public virtual void DraggingStopped()
    {
        if (currentlyDragging)
        {
            Destroy(currentlyDragging.parent.gameObject);
        }
    }

    public virtual void DraggingStarted()
    {
        currentlyDragging = CreateSpriteObject();
    }

    public Transform CreateSpriteObject()
    {
        GameObject canvasObj = new GameObject("drag", typeof(RectTransform), typeof(Canvas));
        GameObject obj = new GameObject("drag", typeof(RectTransform), typeof(Image));
        Image img = obj.GetComponent<Image>();
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
        return spriteForDragging;
    }
}
