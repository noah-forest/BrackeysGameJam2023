using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PopulateEntries : MonoBehaviour
{
    public List<Entry_Tab> tabs = new();
    public List<GameObject> entryContainers = new();
    
    public Color tabIdle;
    public Color tabHover;
    public Color tabActive;

    public Entry_Tab selectedTab;
    public LB_TabManager tabManager;
    
    public GameObject logEntry;

    public Sprite nullSprite;
    
    private UnitManager unitManager;
    private List<BossEncounterSO> bossEncounters;
    
    private void Start()
    {
        unitManager = UnitManager.singleton;
        CreateUnitEntries();
        CreateBossEntries();
    }
    
    private void CreateUnitEntries()
    {
        foreach (var unit in unitManager.unitStatsDatabase)
        {
            var entry = Instantiate(logEntry, entryContainers[0].transform);
            var entryInfo = entry.GetComponent<EntryInfo>();
            entryInfo.entryIcon.sprite = unit.unitSprite;
            entryInfo.entryName = unit.name;
            entryInfo.entryType = EntryType.Unit;
            
            var tab = entry.GetComponent<Entry_Tab>();
            tab.unitInfo = unit;
            tab.manager = this;
        }
    }

    private void CreateBossEntries()
    {
        LoadBossEncounters();
        foreach (var boss in bossEncounters)
        {
            var entry = Instantiate(logEntry, entryContainers[1].transform);
            var entryInfo = entry.GetComponent<EntryInfo>();
            entryInfo.entryIcon.sprite = boss.bossPortrait;
            entryInfo.entryName = boss.bossName;
            entryInfo.entryType = EntryType.Boss;
            
            var tab = entry.GetComponent<Entry_Tab>();
            tab.bossInfo = boss;
            tab.manager = this;
        }
    }

    private void CreateHazardEntries()
    {
        
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

        var desc = tabManager.descToShow[(int)tab.entryInfo.entryType];

        if (tab.entryInfo.entryType == EntryType.Unit)
        {
            var descInfo = desc.GetComponent<UnitsDescInfo>();
            SetUpUnitDesc(descInfo, tab.unitInfo, tab.entryInfo);
        }
        else
        {
            var descInfo = desc.GetComponent<GenericDescInfo>();
            SetUpBossDesc(descInfo, tab.bossInfo, tab.entryInfo);
        }

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

    private void LoadBossEncounters()
    {
        bossEncounters = Resources.LoadAll<BossEncounterSO>("SOs/BossEncounters").ToList();
    }

    private void SetUpBossDesc(GenericDescInfo descInfo, BossEncounterSO bossInfo, EntryInfo entryInfo)
    {
        var headerInfo = tabManager.headerInfo;
        
        if (entryInfo.entryLocked)
        {
            descInfo.description.SetText("Encounter this entry to view more!");
            headerInfo.entryImage.sprite = nullSprite;
            headerInfo.entryName.text = "???";
        }
        else
        {
            descInfo.description.SetText(bossInfo.bossDescription);
            headerInfo.entryImage.sprite = bossInfo.bossPortrait;
            headerInfo.entryName.text = bossInfo.bossName;
        }
        
        headerInfo.header.SetActive(true);
    }

    private void SetUpUnitDesc(UnitsDescInfo descInfo, UnitInfo unitInfo, EntryInfo entryInfo)
    {
        var headerInfo = tabManager.headerInfo;
        
        if (entryInfo.entryLocked)
        {
            headerInfo.entryImage.sprite = nullSprite;
            headerInfo.entryName.text = "???";
            
            descInfo.health.text = $"??";
            descInfo.damage.text = $"??";
            descInfo.digCount.text = $"??";
            descInfo.blockChance.text = $"??";
            descInfo.critChance.text = $"??";
            descInfo.speed.text = "???";
        
            descInfo.description.text = $"Buy this unit to find out!";
            descInfo.rarity.text = $"???";
        }
        else
        {
            headerInfo.entryImage.sprite = unitInfo.unitSprite;
            headerInfo.entryName.text = unitInfo.name;
            
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
        
        headerInfo.header.SetActive(true);
    }
}
