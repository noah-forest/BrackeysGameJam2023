using System.Collections.Generic;
using System.Runtime.InteropServices;
using Assets.Scripts.Units;
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
    
    public GameObject pageCont;
    
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

            var page = Instantiate(tabInfo.page, pageCont.transform);
            var pageInfo = page.GetComponent<PR_PageInfo>();
            if (tabInfo != masterTab)
            {
                page.SetActive(false);
            }
            
            tabInfo.tabGroup = this;

            if (slotItem != null)
            {
                tabInfo.icon.sprite = slotItem.GetSlotSprite();
                FillOutPageInfo(slot, pageInfo);
            }
            
            objectToSwap.Add(page);
            newTabs.Add(temp);
        }
    }

    private void FillOutPageInfo(Slot slot, PR_PageInfo pageInfo)
    {
        var controller = slot.payload.GetComponent<UnitController>();
        var exp = slot.payload.GetComponent<Experience>();
        
        pageInfo.unitName.text = slot.payload.name;
        pageInfo.unitLevel.text = exp.curLevel.ToString();
        OnToggleOff(pageInfo, controller);
        
        pageInfo.allTimeToggle.onToggleOn.AddListener(delegate { OnToggle(pageInfo, controller); });
        pageInfo.allTimeToggle.onToggleOff.AddListener(delegate { OnToggleOff(pageInfo, controller); });
    }

    private void OnToggle(PR_PageInfo pageInfo, UnitController unitController)
    {
        pageInfo.displayCheck.SetText("All Time");
        DisplayUnitPerformance(pageInfo, unitController.unitPerformanceAllTime);
    }

    private void OnToggleOff(PR_PageInfo pageInfo, UnitController unitController)
    {
        pageInfo.displayCheck.SetText("Last Battle");
        DisplayUnitPerformance(pageInfo, unitController.unitPerformanceLastBattle);
    }

    private void DisplayUnitPerformance(PR_PageInfo pageInfo, UnitPerformance performance)
    {
        pageInfo.dmgDealt.SetText($"{performance.damageDealt}");
        pageInfo.dmgToEnemy.SetText($"{performance.damageDealtToActors}");
        pageInfo.dmgToUnits.SetText($"{performance.damageDealtToUnits}");
        pageInfo.dmgBlocked.SetText($"{performance.damageBlocked}");
        pageInfo.dmgReceived.SetText($"{performance.damageRecieved}");
        pageInfo.dmgAllowed.SetText($"{performance.damagePassedToActor}");
        pageInfo.unitsKilled.SetText($"{performance.unitsKilled}");
        pageInfo.actorsKilled.SetText($"{performance.actorsKilled}");
        pageInfo.timesAttacked.SetText($"{performance.timesAttacked}");
        pageInfo.timesCrit.SetText($"{performance.timesCrit}");
        pageInfo.timesDug.SetText($"{performance.timesDug}");
        pageInfo.timesBlocked.SetText($"{performance.timesBlocked}");
        pageInfo.timesDied.SetText($"{performance.timesDied}");
    }
    
    private void DestroyNewTabs()
    {
        if (newTabs.Count == 0) return;
        foreach (var go in newTabs)
        {
            Destroy(go);
        }
        
        newTabs.Clear();
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

        foreach (var go in objectToSwap)
        {
            if (go == master.page) continue;
            Destroy(go);
        }
        
        objectToSwap.Clear();
        tabButtons.Clear();
        
        tabButtons.Add(master);
        objectToSwap.Add(master.page);
        OnTabSelected(master);
    }
    
    public void Subscribe(PR_Tab button)
    {
        tabButtons ??= new List<PR_Tab>();

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
