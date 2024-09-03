using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PortraitManager : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    public GameObject portraitMenu;
    
    public List<PortraitInfo> portraits;

    public Color tabIdle;
    public Color tabHover;
    public Color tabSelected;
    
    public PortraitInfo selectedPortrait;
    
    private MouseUtils mouseUtils;

    public Image currentPortrait;
    
    private bool open;
    void Start()
    {
        mouseUtils = MouseUtils.singleton;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        mouseUtils.SetHoverCursor();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        mouseUtils.SetToDefaultCursor();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        OnClick();
    }
    
    public void Subscribe(PortraitInfo button)
    {
        if (portraits == null)
        {
            portraits = new();
        }

        portraits.Add(button);
    }

    public void OnTabEnter(PortraitInfo button)
    {
        ResetTabs();
        if (selectedPortrait == null || button != selectedPortrait)
        {
            button.portraitBackground.color = tabHover;
        }
    }

    public void OnTabExit(PortraitInfo button)
    {
        ResetTabs();
    }

    public void OnTabSelected(PortraitInfo button)
    {
        selectedPortrait = button;
        ResetTabs();
        button.portraitBackground.color = tabSelected;
        currentPortrait.sprite = button.portraitImage.sprite;
    }

    public void ResetTabs()
    {
        foreach (PortraitInfo button in portraits)
        {
            if (selectedPortrait != null && button == selectedPortrait) continue;
            button.portraitBackground.color = tabIdle;
        }
    }

    public void OnClick()
    {
        if (open)
        {
            open = false;
            portraitMenu.SetActive(false);
        }
        else
        {
            open = true;
            portraitMenu.SetActive(true);
        }
    }
}
