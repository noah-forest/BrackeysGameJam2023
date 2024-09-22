using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class LB_TabManager : MonoBehaviour
{
    public List<LB_Tab> tabs;
    public List<GameObject> objectToSwap;
    public List<GameObject> descToShow;

    public FillOutUnits unitSelector;
    
    public Color tabIdle;
    public Color tabHover;
    public Color tabActive;

    public LB_HeaderInfo headerInfo;
    
    public LB_Tab selectedTab;
    public void Subscribe(LB_Tab tab)
    {
        tabs ??= new List<LB_Tab>();
        
        tabs.Add(tab);
    }

    public void OnTabEnter(LB_Tab tab)
    {
        ResetTabs();
        if (!selectedTab || tab != selectedTab)
        {
            tab.background.color = tabHover;
        }
    }

    public void OnTabExit(LB_Tab tab)
    {
        ResetTabs();
    }

    public void OnTabSelected(LB_Tab tab)
    {
        if (selectedTab != tab)
        {
            descToShow[0].SetActive(false);
            headerInfo.header.SetActive(false);
            unitSelector.selectedTab = null;
            unitSelector.ResetTabs();
        }
        selectedTab = tab;
        ResetTabs();
        tab.background.color = tabActive;
        var index = tab.transform.GetSiblingIndex();
        for (var i = 0; i < objectToSwap.Count; i++)
        {
            objectToSwap[i].SetActive(i == index);
        }
    }

    private void ResetTabs()
    {
        foreach (var tab in tabs)
        {
            if (selectedTab && tab == selectedTab) continue;
            tab.background.color = tabIdle;
        }
    }
}
