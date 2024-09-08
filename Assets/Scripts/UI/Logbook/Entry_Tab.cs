using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Entry_Tab : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    public FillOutUnits manager;
    public Image background;
    
    public UnitInfo unitInfo;
    
    private MouseUtils mouseUtils;
    
    private void Start()
    {
        mouseUtils = MouseUtils.singleton;
        manager.Subscribe(this);
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
