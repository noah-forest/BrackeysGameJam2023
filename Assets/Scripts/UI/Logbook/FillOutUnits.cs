using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FillOutUnits : MonoBehaviour
{
    public List<Entry_Tab> tabs;

    public Color tabIdle;
    public Color tabHover;
    public Color tabActive;

    public Entry_Tab selectedTab;
    public LB_TabManager tabManager;
    
    public GameObject logEntry;
    
    private UnitManager unitManager;

    private void Start()
    {
        unitManager = UnitManager.singleton;
        CreateUnitEntries();
    }
    
    private void CreateUnitEntries()
    {
        foreach (var unit in unitManager.unitStatsDatabase)
        {
            var entry = Instantiate(logEntry, transform);
            var entryInfo = entry.GetComponent<EntryInfo>();
            entryInfo.entryIcon.sprite = unit.unitSprite;
            
            var tab = entry.GetComponent<Entry_Tab>();
            tab.unitInfo = unit;
            tab.manager = this;

            if (!entryInfo.entryLocked)
            {
                entryInfo.lockObj.SetActive(false);
                entryInfo.entryIcon.color = Color.white;
            }
        }
    }

    #region -- tabSystem --

    public void Subscribe(Entry_Tab tab)
    {
        tabs ??= new List<Entry_Tab>();
        
        tabs.Add(tab);
    }

    public void OnTabEnter(Entry_Tab tab)
    {
        ResetTabs();
        if (!selectedTab || tab != selectedTab)
        {
            tab.background.color = tabHover;
        }
    }

    public void OnTabExit(Entry_Tab tab)
    {
        ResetTabs();
    }

    public void OnTabSelected(Entry_Tab tab)
    {
        selectedTab = tab;
        ResetTabs();
        tab.background.color = tabActive;

        var desc = tabManager.descToShow[0]; // this is hard coded and terrible
        var headerInfo = tabManager.headerInfo;
        
        headerInfo.entryImage.sprite = tab.unitInfo.unitSprite;
        headerInfo.entryName.text = tab.unitInfo.name;
        
        headerInfo.header.SetActive(true);
        
        var descInfo = desc.GetComponent<UnitsDescInfo>();
        SetUpDesc(descInfo, tab.unitInfo);
        
        desc.SetActive(true);
    }

    public void ResetTabs()
    {
        foreach (var tab in tabs)
        {
            if (selectedTab && tab == selectedTab) continue;
            tab.background.color = tabIdle;
        }
    }

    #endregion

    private void SetUpDesc(UnitsDescInfo descInfo, UnitInfo unitInfo)
    {
        descInfo.health.text = $"{unitInfo.health}";
        descInfo.damage.text = $"{unitInfo.damage}";
        descInfo.digCount.text = $"{unitInfo.digCount}";
        descInfo.blockChance.text = $"{Mathf.Round(unitInfo.blockChance * 100f)}";
        descInfo.critChance.text = $"{Mathf.Round(unitInfo.critChance * 100f)}";
        
        if ((unitInfo.attackSpeed * 10) > 40) descInfo.speed.text = "Booty";
        else if ((unitInfo.attackSpeed * 10) == 40) descInfo.speed.text = "Slow";
        else if ((unitInfo.attackSpeed * 10) == 20) descInfo.speed.text = "Average";
        else if ((unitInfo.attackSpeed * 10) < 10) descInfo.speed.text = "Nuts";
        else if ((unitInfo.attackSpeed * 10) < 20) descInfo.speed.text = "Fast";
        
        descInfo.description.text = $"{unitInfo.description}";
        descInfo.rarity.text = $"{unitInfo.rarity}";
    }
}
