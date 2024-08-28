using System.Collections.Generic;
using UnityEngine;

public class PR_TabGroup : MonoBehaviour
{
    public List<PR_Tab> tabButtons;
    public List<GameObject> objectToSwap;

    public Color tabIdle;
    public Color tabHover;
    public Color tabActive;

    public PR_Tab selectedTab;

    public PR_Tab masterTab;
    
    [Space(5)]
    public GameObject tabCont;
    public GameObject tab;
    
    private BattleManager battleManager;
    private List<GameObject> newTabs = new();
    
    private void OnEnable()
    {
        battleManager = BattleManager.singleton;
        CreateTabs();
    }

    private void OnDisable()
    {
        DestroyNewTabs();
        DestroyOldTabs(masterTab);
    }
    
    private void CreateTabs()
    {
        foreach (var slot in battleManager.playerBattleSlots)
        {
            if (!slot.payload) continue;
            var temp = Instantiate(tab, tabCont.transform);
            
            var tabInfo = temp.GetComponent<PR_Tab>();
            var slotItem = slot.payload.GetComponent<ISlotItem>();

            tabInfo.tabGroup = this;
            
            if (slotItem != null) 
                tabInfo.icon.sprite = slotItem.GetSlotSprite();
            
            newTabs.Add(temp);
        }
    }

    private void DestroyNewTabs()
    {
        if (newTabs.Count == 0) return;
        foreach (var go in newTabs)
        {
            Destroy(go);
        }
        newTabs.Clear();
        newTabs.TrimExcess();
    }

    private void DestroyOldTabs(PR_Tab master)
    {
        if (tabButtons.Count <= 1) return;
        foreach (var prTab in tabButtons)
        {
            if (prTab == master) continue;
            var temp = prTab;
            Destroy(temp);
        }

        tabButtons.Clear();
        
        tabButtons.Add(master);
        OnTabSelected(master);
    }
    
    public void Subscribe(PR_Tab button)
    {
        if (tabButtons == null)
        {
            tabButtons = new List<PR_Tab>();
        }
        
        tabButtons.Add(button);
    }

    public void OnTabEnter(PR_Tab button)
    {
        ResetTabs();
        if (!selectedTab || button != selectedTab)
        {
            button.background.color = tabHover;
        }
    }

    public void OnTabExit(PR_Tab button)
    {
        ResetTabs();
    }

    public void OnTabSelected(PR_Tab button)
    {
        selectedTab = button;
        ResetTabs();
        button.background.color = tabActive;
        var index = button.transform.GetSiblingIndex();
        for (var i = 0; i < objectToSwap.Count; ++i)
        {
            objectToSwap[i].SetActive(i == index);
        }
    }
    
    private void ResetTabs()
    {
        foreach (var button in tabButtons)
        {
            if (selectedTab && button == selectedTab) continue;
            button.background.color = tabIdle;
        }
    }
}
