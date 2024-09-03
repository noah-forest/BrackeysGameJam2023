using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PortraitInfo : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler, IPointerExitHandler
{
    public bool firstSelected;
    public PortraitManager manager;
    
    public Image portraitImage;
    public Image portraitBackground;
    
    private Button thisButton;
    private MouseUtils mouseUtils;

    private void Start()
    {
        mouseUtils = MouseUtils.singleton;
        
        manager.Subscribe(this);
        if (firstSelected)
        {
            manager.OnTabSelected(this);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        mouseUtils.SetHoverCursor();
        manager.OnTabEnter(this);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        manager.OnTabSelected(this);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        mouseUtils.SetToDefaultCursor();
        manager.OnTabExit(this);
    }
}
