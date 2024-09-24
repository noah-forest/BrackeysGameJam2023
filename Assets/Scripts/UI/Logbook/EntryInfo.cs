using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class EntryInfo : MonoBehaviour
{
    // takes in a name of the thing to unlock
    public static UnityEvent<string> onEntryUnlocked = new();
    
    public string entryName;
    public Image entryIcon;
    public GameObject lockObj;
    public bool entryLocked = true;

    private void Start()
    {
        onEntryUnlocked.AddListener(UnlockEntry);
    }

    private void UnlockEntry(string name)
    {
        if (!entryLocked) return;
        
        if (entryName == name)
        {
            entryLocked = false;
            lockObj.SetActive(false);
            entryIcon.color = Color.white;
        }
    }
}
