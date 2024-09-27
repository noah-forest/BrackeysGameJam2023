using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class PopulateEntries : MonoBehaviour
{
    public List<Entry_Tab> tabs = new();
    public List<GameObject> entryContainers = new();

	public static UnityEvent unlockedEntriesChanged = new();
    
    public Color tabIdle;
    public Color tabHover;
    public Color tabActive;

	public TextMeshProUGUI completionText;

    public Entry_Tab selectedTab;
    public LB_TabManager tabManager;
    public GameObject logEntry;
    public Sprite nullSprite;

	private UnitManager unitManager;
	private SaveData saveData;
    private List<BossEncounterSO> bossEncounters;
    private List<HazardEntrySO> hazardEncounters;

	private float numOfEntries;
	private float numOfUnlockedEntries;

	private void Start()
    {
        unitManager = UnitManager.singleton;
		saveData = SaveData.singleton;

		CreateUnitEntries();
        CreateBossEntries();
        CreateHazardEntries();

        numOfEntries =
            unitManager.unitStatsDatabase.Count + bossEncounters.Count + hazardEncounters.Count;

		unlockedEntriesChanged.AddListener(UpdateCompletion);

		LoadCompletion(saveData.unlockMatrix.unitsUnlocked);
		LoadCompletion(saveData.unlockMatrix.bossesUnlocked);
		LoadCompletion(saveData.unlockMatrix.hazardsUnlocked);
		CheckCompletion();
	}

	private void LoadCompletion(List<string> unlockedEntries)
	{
		if (unlockedEntries.Count <= 0) return;
		foreach (var entry in unlockedEntries)
		{
			unlockedEntriesChanged.Invoke();
		}
	}

	private void CheckCompletion()
	{
		completionText.text = $"Completed: {MathF.Round(numOfUnlockedEntries / numOfEntries * 100, 1)}%";
	}

	private void UpdateCompletion()
	{
		++numOfUnlockedEntries;
		completionText.text = $"Completed: {MathF.Round(numOfUnlockedEntries / numOfEntries * 100, 1)}%";
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
        LoadHazardEncounters();
        foreach (var hazard in hazardEncounters)
        {
            var entry = Instantiate(logEntry, entryContainers[2].transform);
            var entryInfo = entry.GetComponent<EntryInfo>();
            entryInfo.entryIcon.sprite = hazard.hazardIcon;
            entryInfo.entryName = hazard.hazardName;
            entryInfo.entryType = EntryType.Hazard;
            
            var tab = entry.GetComponent<Entry_Tab>();
            tab.hazardInfo = hazard;
            tab.manager = this;
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

        var desc = tabManager.descToShow[(int)tab.entryInfo.entryType];

        switch (tab.entryInfo.entryType)
        {
            case EntryType.Unit:
            {
                var descInfo = desc.GetComponent<UnitsDescInfo>();
                SetUpUnitDesc(descInfo, tab.unitInfo, tab.entryInfo);
                break;
            }
            case EntryType.Boss:
            {
                var descInfo = desc.GetComponent<GenericDescInfo>();
                SetUpBossDesc(descInfo, tab.bossInfo, tab.entryInfo);
                break;
            }
            case EntryType.Hazard:
            {
                var descInfo = desc.GetComponent<GenericDescInfo>();
                SetUpHazardDesc(descInfo, tab.hazardInfo, tab.entryInfo);
                break;
            }
            default:
                throw new ArgumentOutOfRangeException();
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

    private void LoadHazardEncounters()
    {
        hazardEncounters = Resources.LoadAll<HazardEntrySO>("SOs/Hazards").ToList();
    }

    private void SetUpHazardDesc(GenericDescInfo descInfo, HazardEntrySO hazardInfo, EntryInfo entryInfo)
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
            descInfo.description.SetText(hazardInfo.hazardDescription);
            headerInfo.entryImage.sprite = hazardInfo.hazardIcon;
            headerInfo.entryName.text = hazardInfo.hazardName;
        }
        
        headerInfo.header.SetActive(true);
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
