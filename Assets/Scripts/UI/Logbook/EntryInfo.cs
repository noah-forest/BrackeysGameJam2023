using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public enum EntryType
{
    Unit,
    Boss,
    Hazard
}

public class EntryInfo : MonoBehaviour
{
    // takes in a name of the thing to unlock
    public static UnityEvent<string> onEntryUnlocked = new();
    
    public string entryName;
    public Image entryIcon;
    public GameObject lockObj;
    public bool entryLocked = true;
    public EntryType entryType;
    
    private SaveData saveData;
    
    private void Start()
    {
        saveData = SaveData.singleton;

        //check if something was unlocked before the event can be listened to
        LoadUnlockedEntries(saveData.unlockMatrix.unitsUnlocked);
        LoadUnlockedEntries(saveData.unlockMatrix.bossesUnlocked);
        LoadUnlockedEntries(saveData.unlockMatrix.hazardsUnlocked);
        
        onEntryUnlocked.AddListener(UnlockEntry);
    }

    private void LoadUnlockedEntries(List<string> unlockedEntries)
    {
        if (unlockedEntries.Count <= 0 || !entryLocked) return;
        foreach (var entry in unlockedEntries)
        {
			UnlockEntry(entry);
        }
    }

    private void UnlockEntry(string name)
    {
        if (!entryLocked) return;
        if (entryName != name) return;
        
        entryLocked = false;
        lockObj.SetActive(false);
        entryIcon.color = Color.white;
        
        // save the unlock matrix when a new unit is unlocked
        saveData.SaveIntoJson();
    }
}
